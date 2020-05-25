using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Threading.Tasks;

namespace DocumentClientSQL
{
    class SQL_Sprocs
    {
        private DocumentClient client;
        private Uri collectionLink;
        private string dbName, collName;

        public SQL_Sprocs(String dbName, String collName, DocumentClient client)
        {
            this.collectionLink = UriFactory.CreateDocumentCollectionUri(dbName, collName);
            this.dbName = dbName;
            this.collName = collName;
            this.client = client;
        }

        public async Task createSproc(string sprocId, string sprocBody) // Creating a Stored Procedure
        {
            try
            {
                ResourceResponse<StoredProcedure> sproc = await this.client.CreateStoredProcedureAsync(collectionLink, new StoredProcedure
                {
                    Id = sprocId,
                    Body = sprocBody,
                });
                Console.WriteLine("Created Stored procedure " + sproc.Resource.Id + ":\n" + sproc.Resource.Body +"\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public async Task readSproc(string sprocId)  // Reading a Stored Procedure
        {
            Uri sprocLink = UriFactory.CreateStoredProcedureUri(dbName, collName, sprocId);
            try
            {
                var sproc = await this.client.ReadStoredProcedureAsync(sprocLink);
                Console.WriteLine("Reading Sproc " + sproc.Resource.Id + ":\n" +sproc.Resource.Body + "\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
		//Testing Text here
        public async Task updateSproc(string sprocId, string sprocBody) // Updating a Stored Procedure
        {
            Uri sprocLink = UriFactory.CreateStoredProcedureUri(dbName, collName, sprocId);
            ResourceResponse<StoredProcedure> sprocReplace;

            var sproc = new StoredProcedure()
            {
                Body = sprocBody,
                Id = sprocId
            };

            try
            {
                sprocReplace = await client.ReplaceStoredProcedureAsync(sprocLink, sproc);
                Console.WriteLine("Updated Stored procedure " + sprocReplace.Resource.Id + " to\n" + sprocReplace.Resource.Body + "\n");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task executeSproc(string sprocId) //Executing a Stored Procedure
        {
            String[] arr = new String[]{ "Microsoft" };
            RequestOptions options = new RequestOptions()
            {
                PartitionKey = new PartitionKey("6")
            };
            StoredProcedureResponse<string> sprocResp = await this.client.ExecuteStoredProcedureAsync<string>(UriFactory.CreateStoredProcedureUri(this.dbName, this.collName, sprocId), options, arr);
            Console.WriteLine(sprocResp.Response + "\n");
        }

        public async Task deleteSproc(string sprocId) //Deleting a Stored Procedure
        {
            Uri sprocLink = UriFactory.CreateStoredProcedureUri(this.dbName, this.collName, sprocId);
            Console.WriteLine("Deleting the Stored Procedure " + sprocId + "\n");
            await this.client.DeleteStoredProcedureAsync(sprocLink);
        }
    }
}
