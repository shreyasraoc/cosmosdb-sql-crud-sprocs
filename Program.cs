using Microsoft.Azure.Documents.Client;
using System;


namespace DocumentClientSQL
{
    class Program
    {

        static void Main(string[] args)
        {
            ConnectionPolicy connectionPolicy = new ConnectionPolicy();
            connectionPolicy.ConnectionMode = ConnectionMode.Direct;
            connectionPolicy.ConnectionProtocol = Protocol.Tcp;

            DocumentClient client = new DocumentClient(new Uri("<ENDPOINT_FROM_PORTAL>"),
               "PRIMARY_KEY_FROM_PORTAL");
            /*
             * 
             * Creating and Updating Stored Procedures in SQL API CosmosDB. Need to still DELETE, EXECUTE
             * This code displays the sproc function body after create or update.
             * 
             */
            SQL_Sprocs sprocs = new SQL_Sprocs("city_docdb", "city_coll", client);
            string sprocId = "testSproc_1";

            //Modify the Values in the Above Object creation call and sproc Name
            //Create the Sproc
            sprocs.createSproc(sprocId, "function () { getContext().getResponse().setBody('Hello Bob!'); }").Wait();

            //Read a Stored Procedure
            sprocs.readSproc(sprocId).Wait();

            //Update the Sproc
            sprocs.updateSproc(sprocId, "function (text) { getContext().getResponse().setBody('Hello ' + text + '!!'); }").Wait();

            //Execute the Sproc
            sprocs.executeSproc(sprocId).Wait();

            //Delete a Sproc
            sprocs.deleteSproc(sprocId).Wait();
            
            /*
             * 
             * All CRUD Operations from here
             * Documents are under docObject class(format is available there)
             * Use upserts for inserts as well
             * 
             */
            SQL_CRUD sqlCrud = new SQL_CRUD(client, "city_docdb", "city_coll");

            //Creating a Database
            sqlCrud.createDatabaseIfNotExists().Wait();

            //Creating a Collection
            sqlCrud.createCollectionIfNotExists(400).Wait();

            //Executing a Query
            sqlCrud.executeSqlQuery("SELECT COUNT(1) as Value_1 FROM c").Wait();

            //Upserting/Inserting a document( Run over a for loop to Insert/Upsert a lot of docs or use the Bulk Executor's BulkUpdate API )
            docObject doc = new docObject()
            {
                city = "Bangalore",
                id = "6",
                Value_1 = 22,
                Value_2 = 10
            };
            sqlCrud.upsertDocuments(doc).Wait();

            //Deleting a document ( Run over a for loop to delete a lot of docs or use the Bulk Executor BulkDelete API )
            sqlCrud.deleteDocuments("6", "Bangalore").Wait();
            
            Console.ReadKey();
            

        }

    }
}
