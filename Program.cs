using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Serilog;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace docker_sound_dl
{
    class Program
    {
        private static ILogger logger;
        private static BlobContainerClient containerClient;

        public static string YtdlPath { get; set; } = "/usr/bin/yt-dlp";

        static async Task Main()
        {
            // 建立Logger
            Log.Logger = new LoggerConfiguration()
                            .MinimumLevel.Verbose()
                            .WriteTo.Console()
                            .CreateLogger();
            logger = Log.Logger;

            // 同步執行
            bool sync = null != Environment.GetEnvironmentVariable("SCYNCHRONOUS");

            // 計時
            DateTime startTime = DateTime.Now;
            logger.Information("Start sound-dl {now}", startTime.ToString());

            // Create a BlobServiceClient object which will be used to create a container client
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            BlobServiceClient blobServiceClient = new(connectionString);
            containerClient = blobServiceClient.GetBlobContainerClient("sound-buttons");

            // 取得要下載的連結
            string channelsToDownload = Environment.GetEnvironmentVariable("CHANNELS_IN_ARRAY");
            string[] channels = JsonSerializer.Deserialize<string[]>(channelsToDownload);

            // TempPath
            string tempDir = Path.Combine(Path.GetTempPath(), "audio-dl");
            _ = Directory.CreateDirectory(tempDir);
            string archivePath = Path.Combine(Path.GetTempPath(), "archive.txt");

            try
            {
                // 取得archive.txt
                BlobClient archiveBlob = containerClient.GetBlobClient("AudioSource/archive.txt");
                if (archiveBlob.Exists())
                {
                    _ = archiveBlob.DownloadTo(archivePath);
                }

                OptionSet optionSet = new()
                {
                    Format = "251",
                    NoCheckCertificate = true,
                    Output = Path.Combine(tempDir, "%(id)s"),
                    DownloadArchive = archivePath,
                    Continue = true,
                    IgnoreErrors = true,
                    NoOverwrites = true
                };

                YoutubeDLProcess ytdlProc = new(YtdlPath);
                ytdlProc.OutputReceived += (o, e) => logger.Verbose(e.Data);
                ytdlProc.ErrorReceived += (o, e) => logger.Error(e.Data);

                logger.Information("Start download process.");

                // 下載音訊
                await ytdlProc.RunAsync(
                    channels,
                    optionSet,
                    new System.Threading.CancellationToken());

                // 上傳blob storage
                List<Task> tasks = new();
                foreach (string filePath in Directory.GetFiles(tempDir))
                {
                    Task<bool> task = UploadToAzure(filePath);
                    tasks.Add(task);
                    if (sync) await task;
                }
                tasks.Add(UploadToAzure(archivePath, ContentType: "text/plain"));

                await Task.WhenAll(tasks.ToArray());
                logger.Debug("All tasks are completed. Total time spent: {timeSpent}", (DateTime.Now - startTime).ToString("hh\\:mm\\:ss"));
            }
            finally
            {
                Directory.Delete(tempDir, true);
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// 上傳檔案至Azure Blob Storage
        /// </summary>
        /// <param name="containerClient"></param>
        /// <param name="tempDir">用來計算Storage內路徑的基準路徑</param>
        /// <param name="filePath">上傳檔案路徑</param>
        /// <returns></returns>
        private static async Task<bool> UploadToAzure(string filePath, bool retry = true, string ContentType = "audio/webm")
        {
            bool isVideo = ContentType == "audio/webm";
            try
            {
                using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
                logger.Debug("Start Upload {path} to azure storage", filePath);

                long fileSize = new FileInfo(filePath).Length;

                // 覆寫
                _ = await containerClient
                    .GetBlobClient($"AudioSource/{Path.GetFileName(filePath)}")
                    .UploadAsync(content: fs,
                                 httpHeaders: new BlobHttpHeaders { ContentType = ContentType },
                                 accessTier: AccessTier.Hot,
                                 progressHandler: new Progress<long>(progress =>
                                 {
                                     logger.Verbose("Uploading...{progress}% {path}", Math.Round(((double)progress) / fileSize * 100), filePath);
                                 }));
                logger.Debug("Finish Upload {path} to azure storage", filePath);

                if (isVideo) File.Delete(filePath);
                return true;
            }
            catch (Exception e)
            {
                if (retry)
                {
                    // Retry Once
                    return await UploadToAzure(filePath, false);
                }
                else
                {
                    logger.Error("Upload Failed: {fileName}", Path.GetFileName(filePath));
                    logger.Error("{errorMessage}", e.Message);
                    return false;
                }
            }
        }
    }
}
