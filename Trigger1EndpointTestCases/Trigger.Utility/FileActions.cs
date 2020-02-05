using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Trigger.Utility
{
    public static class FileActions
    {
        public static readonly IConfiguration _iconfiguration;
        private const string slash = "/";
        private const string dollarSign = "$";

        public static void UploadProfilePic(string destinationFolderPath, string imageName, string imageBytes)
        {
            byte[] bytes = System.Convert.FromBase64String(imageBytes);
            if (!Directory.Exists(destinationFolderPath))
                Directory.CreateDirectory(destinationFolderPath);

            MemoryStream empImageStream = new MemoryStream(bytes);
            FileStream empImageFile = new FileStream(destinationFolderPath + slash + imageName, FileMode.Create, FileAccess.Write);
            empImageStream.WriteTo(empImageFile);
            empImageFile.Close();
            empImageStream.Close();
        }

        public static void DeleteProfilePic(string destinationFolderPath, string imageName)
        {
            if (Directory.Exists(destinationFolderPath) && File.Exists(destinationFolderPath + imageName))
            {
                File.Delete(destinationFolderPath + imageName);
            }
        }

        public static async Task UploadtoBlobAsync(string imageName, string imageBytes, string storageAccountName, string storageAccountAccessKey, string blobContainer)
        {
            byte[] bytes = Convert.FromBase64String(imageBytes);
            Stream stream = new MemoryStream(bytes);
            await UploadFileToStorage(stream, imageName, storageAccountName, storageAccountAccessKey, blobContainer);
        }

        public static async Task<bool> UploadFileToStorage(Stream fileStream, string fileName, string storageAccountName, string storageAccountAccessKey, string blobContainer)
        {
            try
            {
                StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountAccessKey);
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);
                await blockBlob.UploadFromStreamAsync(fileStream);

                return await Task.FromResult(true);
            }
            catch (Exception)
            {
                return await Task.FromResult(false);
            }
        }

        /// <summary>
        /// Created By : Vivek Bhavsar
        /// Purpose    : Method to Delete File from blob storage
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="storageAccountName"></param>
        /// <param name="storageAccountAccessKey"></param>
        /// <param name="blobContainer"></param>
        /// <returns></returns>
        public static async Task<int> DeleteFileFromBlobStorage(string fileName, string storageAccountName, string storageAccountAccessKey, string blobContainer)
        {
            try
            {
                StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountAccessKey);
                CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(blobContainer);

                var blob = container.GetBlockBlobReference(fileName);
                await blob.DeleteIfExistsAsync();

                return await Task.FromResult(1);
            }
            catch (Exception)
            {
                return await Task.FromResult(0);
            }
        }

        public static string EvalNullString(string field, string optional)
        {
            if (field == null)
            {
                field = optional;
            }
            return field;
        }

        /// <summary>
        /// return file name with concatation of unique guid & documentname with $ seperater
        /// </summary>
        /// <param name="documentName"></param>
        /// <returns></returns>
        public static string GenerateUniqueDocumentName(string documentName)
        {
            return documentName == string.Empty ? string.Empty : Guid.NewGuid().ToString() + dollarSign + documentName;
        }
    }
}
