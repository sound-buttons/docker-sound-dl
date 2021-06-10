# docker-sound-dl

![License](https://img.shields.io/github/license/jim60105/sound-buttons?style=for-the-badge)
![.NET Core](https://img.shields.io/static/v1?style=for-the-badge&message=.NET+Core&color=512BD4&logo=.NET&logoColor=FFFFFF&label=)
![Microsoft Azure](https://img.shields.io/static/v1?style=for-the-badge&message=Microsoft+Azure&color=0078D4&logo=Microsoft+Azure&logoColor=FFFFFF&label=)
![Docker](https://img.shields.io/static/v1?style=for-the-badge&message=Docker&color=2496ED&logo=Docker&logoColor=FFFFFF&label=)
![YouTube](https://img.shields.io/static/v1?style=for-the-badge&message=YouTube&color=FF0000&logo=YouTube&logoColor=FFFFFF&label=)

這是[sound-buttons](https://github.com/jim60105/sound-buttons)專案的一部份，排程每天先下載完整音源上傳至Azure Blob Storage儲存\
這是一支 .Net Core Console Application，並包裝為Linux Container，掛在我的個人主機排程執行docker run

## Setting up

- 在本機環境變數中儲存connection string，命名為「AZURE_STORAGE_CONNECTION_STRING」\
<https://docs.microsoft.com/zh-tw/azure/storage/common/storage-account-keys-manage?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&tabs=azure-portal#view-account-access-keys>
- docker run

        docker run --rm --env CHANNELS_IN_ARRAY="[\"https://www.youtube.com/channel/UCBC7vYFNQoGPupe5NxPG4Bw\", \"https://www.youtube.com/channel/UC7XCjKxBEct0uAukpQXNFPw\", \"https://www.youtube.com/channel/UCuy-kZJ7HWwUU-eKv0zUZFQ\"]" --env AZURE_STORAGE_CONNECTION_STRING jim60105/docker-sound-dl

## LICENSE

- Distribute main code with MIT License.
- Use YoutubeDLSharp under BSD 3-Clause License.
- Use youtube-dl under Unlicensed License.
