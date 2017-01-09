using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Azure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage.Shared.Protocol;
using Microsoft.WindowsAzure.Storage.File.Protocol;

namespace AzureTableStorage.Models
{
    public class MyFileStorage
    {
        private string Account { get; set; }
        private string Key { get; set; }

        public MyFileStorage()
        {
            this.Account = "testaccounttoday";
            this.Key = "Z/ddM9f7mIl1bm9yVRnzmENUPbW6yg8o0zEMgRtK4Mn1iSau5y4xuwK7pe2z/nzzxhwBFt4OTBLeSBu7+hycAA==";
        }

        public void Access_the_file_share_programmatically()
        {
            // Retrieve storage account from connection string.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create a CloudFileClient object for credentialed access to File storage.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference("logs");

            // Ensure that the share exists.
            if (share.Exists())
            {
                // Get a reference to the root directory for the share.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("CustomLogs");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile file = sampleDir.GetFileReference("Log1.txt");

                    // Ensure that the file exists.
                    if (file.Exists())
                    {
                        // Write the contents of the file to the console window.
                        Console.WriteLine(file.DownloadTextAsync().Result);
                    }
                }
            }
        }

        public void Set_the_maximum_size_for_a_file_share()
        {
            // Parse the connection string for the storage account.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create a CloudFileClient object for credentialed access to File storage.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference("logs");

            // Ensure that the share exists.
            if (share.Exists())
            {
                // Check current usage stats for the share.
                // Note that the ShareStats object is part of the protocol layer for the File service.
                Microsoft.WindowsAzure.Storage.File.Protocol.ShareStats stats = share.GetStats();
                Console.WriteLine("Current share usage: {0} GB", stats.Usage.ToString());

                // Specify the maximum size of the share, in GB.
                // This line sets the quota to be 10 GB greater than the current usage of the share.
                share.Properties.Quota = 10 + stats.Usage;
                share.SetProperties();

                // Now check the quota for the share. Call FetchAttributes() to populate the share's properties.
                share.FetchAttributes();
                Console.WriteLine("Current share quota: {0} GB", share.Properties.Quota);
            }
        }

        public void Generate_a_shared_access_signature_for_a_file_or_file_share()
        {
            // Parse the connection string for the storage account.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create a CloudFileClient object for credentialed access to File storage.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference("logs");

            // Ensure that the share exists.
            if (share.Exists())
            {
                string policyName = "sampleSharePolicy" + DateTime.UtcNow.Ticks;

                // Create a new shared access policy and define its constraints.
                SharedAccessFilePolicy sharedPolicy = new SharedAccessFilePolicy()
                {
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24),
                    Permissions = SharedAccessFilePermissions.Read | SharedAccessFilePermissions.Write
                };

                // Get existing permissions for the share.
                FileSharePermissions permissions = share.GetPermissions();

                // Add the shared access policy to the share's policies. Note that each policy must have a unique name.
                permissions.SharedAccessPolicies.Add(policyName, sharedPolicy);
                share.SetPermissions(permissions);

                // Generate a SAS for a file in the share and associate this access policy with it.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("CustomLogs");
                CloudFile file = sampleDir.GetFileReference("Log1.txt");
                string sasToken = file.GetSharedAccessSignature(null, policyName);
                Uri fileSasUri = new Uri(file.StorageUri.PrimaryUri.ToString() + sasToken);

                // Create a new CloudFile object from the SAS, and write some text to the file.
                CloudFile fileSas = new CloudFile(fileSasUri);
                fileSas.UploadText("This write operation is authenticated via SAS.");
                Console.WriteLine(fileSas.DownloadText());
            }
        }

        public void Copy_file_to_another_file()
        {
            // Parse the connection string for the storage account.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create a CloudFileClient object for credentialed access to File storage.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Get a reference to the file share we created previously.
            CloudFileShare share = fileClient.GetShareReference("logs");

            // Ensure that the share exists.
            if (share.Exists())
            {
                // Get a reference to the root directory for the share.
                CloudFileDirectory rootDir = share.GetRootDirectoryReference();

                // Get a reference to the directory we created previously.
                CloudFileDirectory sampleDir = rootDir.GetDirectoryReference("CustomLogs");

                // Ensure that the directory exists.
                if (sampleDir.Exists())
                {
                    // Get a reference to the file we created previously.
                    CloudFile sourceFile = sampleDir.GetFileReference("Log1.txt");

                    // Ensure that the source file exists.
                    if (sourceFile.Exists())
                    {
                        // Get a reference to the destination file.
                        CloudFile destFile = sampleDir.GetFileReference("Log1Copy.txt");

                        // Start the copy operation.
                        destFile.StartCopy(sourceFile);

                        // Write the contents of the destination file to the console window.
                        Console.WriteLine(destFile.DownloadText());
                    }
                }
            }
        }

        public void Copy_a_file_to_a_blob()
        {
            // Parse the connection string for the storage account.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create a CloudFileClient object for credentialed access to File storage.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Create a new file share, if it does not already exist.
            CloudFileShare share = fileClient.GetShareReference("sample-share");
            share.CreateIfNotExists();

            // Create a new file in the root directory.
            CloudFile sourceFile = share.GetRootDirectoryReference().GetFileReference("sample-file.txt");
            sourceFile.UploadText("A sample file in the root directory.");

            // Get a reference to the blob to which the file will be copied.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("sample-container");
            container.CreateIfNotExists();
            CloudBlockBlob destBlob = container.GetBlockBlobReference("sample-blob.txt");

            // Create a SAS for the file that's valid for 24 hours.
            // Note that when you are copying a file to a blob, or a blob to a file, you must use a SAS
            // to authenticate access to the source object, even if you are copying within the same
            // storage account.
            string fileSas = sourceFile.GetSharedAccessSignature(new SharedAccessFilePolicy()
            {
                // Only read permissions are required for the source file.
                Permissions = SharedAccessFilePermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(24)
            });

            // Construct the URI to the source file, including the SAS token.
            Uri fileSasUri = new Uri(sourceFile.StorageUri.PrimaryUri.ToString() + fileSas);

            // Copy the file to the blob.
            destBlob.StartCopy(fileSasUri);

            // Write the contents of the file to the console window.
            Console.WriteLine("Source file contents: {0}", sourceFile.DownloadText());
            Console.WriteLine("Destination blob contents: {0}", destBlob.DownloadText());
        }

        public void Troubleshooting_File_storage_using_metrics()
        {
            // Parse your storage connection string from your application's configuration file.
            StorageCredentials Credentials = new StorageCredentials(this.Account, this.Key);
            CloudStorageAccount storageAccount = new CloudStorageAccount(Credentials, false);

            // Create the File service client.
            CloudFileClient fileClient = storageAccount.CreateCloudFileClient();

            // Set metrics properties for File service.
            // Note that the File service currently uses its own service properties type,
            // available in the Microsoft.WindowsAzure.Storage.File.Protocol namespace.
            fileClient.SetServiceProperties(new FileServiceProperties()
            {
                // Set hour metrics
                HourMetrics = new MetricsProperties()
                {
                    MetricsLevel = MetricsLevel.ServiceAndApi,
                    RetentionDays = 14,
                    Version = "1.0"
                },
                // Set minute metrics
                MinuteMetrics = new MetricsProperties()
                {
                    MetricsLevel = MetricsLevel.ServiceAndApi,
                    RetentionDays = 7,
                    Version = "1.0"
                }
            });

            // Read the metrics properties we just set.
            FileServiceProperties serviceProperties = fileClient.GetServiceProperties();
            Console.WriteLine("Hour metrics:");
            Console.WriteLine(serviceProperties.HourMetrics.MetricsLevel);
            Console.WriteLine(serviceProperties.HourMetrics.RetentionDays);
            Console.WriteLine(serviceProperties.HourMetrics.Version);
            Console.WriteLine();
            Console.WriteLine("Minute metrics:");
            Console.WriteLine(serviceProperties.MinuteMetrics.MetricsLevel);
            Console.WriteLine(serviceProperties.MinuteMetrics.RetentionDays);
            Console.WriteLine(serviceProperties.MinuteMetrics.Version);
        }
    }
}