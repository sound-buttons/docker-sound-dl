# docker-sound-dl

![License](https://img.shields.io/github/license/jim60105/sound-buttons?style=for-the-badge)
![.NET Core](https://img.shields.io/static/v1?style=for-the-badge&message=.NET+Core&color=512BD4&logo=.NET&logoColor=FFFFFF&label=)
![Microsoft Azure](https://img.shields.io/static/v1?style=for-the-badge&message=Microsoft+Azure&color=0078D4&logo=Microsoft+Azure&logoColor=FFFFFF&label=)
![Docker](https://img.shields.io/static/v1?style=for-the-badge&message=Docker&color=2496ED&logo=Docker&logoColor=FFFFFF&label=)
![YouTube](https://img.shields.io/static/v1?style=for-the-badge&message=YouTube&color=FF0000&logo=YouTube&logoColor=FFFFFF&label=)

這是[sound-buttons](https://github.com/jim60105/sound-buttons)專案的一部份，排程每天先下載完整音源上傳至Azure Blob Storage儲存\
這是一支 .NET 6 Console Application，並包裝為Linux Container，掛在我的個人主機排程執行docker run

## Setting up

請參考 [`.env_sample`](.env_sample) 建立 `.env`

* `CHANNELS_IN_ARRAY`=要快取的影片channel
* `AZURE_STORAGE_CONNECTION_STRING`=Azure storage連接字串，詳見[官方文件](https://docs.microsoft.com/zh-tw/azure/storage/common/storage-account-keys-manage?toc=%2Fazure%2Fstorage%2Fblobs%2Ftoc.json&tabs=azure-portal#view-account-access-keys)
* `LOGSERVER`=GELF log server位置，詳細請參照後文

## LICENSE

* Distribute main code with MIT License.
* Use YoutubeDLSharp under BSD 3-Clause License.
* Use youtube-dl under Unlicensed License.
