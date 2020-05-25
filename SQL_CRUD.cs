using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentClientSQL
{
    class SQL_CRUD
    {
        private DocumentClient client;
        private string dbName;
        private string collName;
        private docObject doc;

        public SQL_CRUD(DocumentClient client, String dbName, String collName, docObject doc = null)
        {
            this.client = client;
            this.dbName = dbName;
            this.collName = collName;
            this.doc = doc;
        }

        public async Task createDatabaseIfNotExists()    //For Database Creation
        {
            Database db = new Database();
            db.Id = this.dbName;

            ResourceResponse<Database> crDB;
            try
            {
                crDB = await this.client.CreateDatabaseIfNotExistsAsync(db);
                Console.WriteLine(crDB.StatusCode);
                Console.WriteLine(crDB.Resource.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task createCollectionIfNotExists(int throughput)     //For Collection Creation
        {
            Uri dbUri = UriFactory.CreateDatabaseUri(this.dbName);

            PartitionKeyDefinition partKey = new PartitionKeyDefinition();
            Collection<String> partPath = new Collection<string>();
            partPath.Add("/test_PartKey");

            partKey.Paths = partPath;

            DocumentCollection docColl = new DocumentCollection();
            docColl.Id = this.collName;
            docColl.PartitionKey = partKey;

            RequestOptions options = new RequestOptions();
            options.OfferThroughput = throughput;

            ResourceResponse<DocumentCollection> crColl;
            try
            {
                crColl = await this.client.CreateDocumentCollectionIfNotExistsAsync(dbUri, docColl, options);
                Console.WriteLine(crColl.StatusCode);
                Console.WriteLine(crColl.RequestDiagnosticsString);
                Console.WriteLine(crColl.Resource.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task executeSqlQuery(string query)    //For Runnning a SQL Query on the Collection. Can also be used to Read the collections.
        {
            Uri docUri = UriFactory.CreateDocumentCollectionUri(this.dbName, this.collName);

            FeedOptions options = new FeedOptions();
            options.EnableCrossPartitionQuery = true;

            IDocumentQuery<docObject> queryable = this.client.CreateDocumentQuery<docObject>(docUri, query, options).AsDocumentQuery();


            Console.WriteLine("\tID\tValue-1\tCity");
            while (queryable.HasMoreResults)
            {
                FeedResponse<docObject> result = await queryable.ExecuteNextAsync<docObject>();
                List<docObject> resList = result.ToList<docObject>();
                foreach (var resPrint in resList)
                {
                    Console.WriteLine("\t" + resPrint.id + "\t" + resPrint.Value_1 + "\t" + resPrint.city);
                }
                //Get Diagnostics Information
                Console.WriteLine(result.RequestDiagnosticsString);
            }
        }

        public async Task upsertDocuments(docObject doc)  //To Upsert/Insert Documents Documents to the CosmosDB
        {
            Uri docUri = UriFactory.CreateDocumentCollectionUri(dbName, collName);
            try
            {
                ResourceResponse<Document> response = await client.UpsertDocumentAsync(docUri, doc);
                Console.WriteLine(response.RequestDiagnosticsString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task deleteDocuments(string id, string partitionKeyValue) //Deleting the documents with id and partition key value
        {
            Uri docUri = UriFactory.CreateDocumentUri(dbName, collName, id);
            try
            {
                ResourceResponse<Document> response = await client.DeleteDocumentAsync(docUri, new RequestOptions()
                {
                    PartitionKey = new PartitionKey(partitionKeyValue)
                });
                Console.WriteLine(response.RequestDiagnosticsString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task indexTransform()
        {
            // set the RequestOptions object

            RequestOptions requestOptions = new RequestOptions();
            requestOptions.PopulateQuotaInfo = true;
            // retrieve the container's details
            Uri docCollUri = UriFactory.CreateDocumentCollectionUri(this.dbName, this.collName);


            ResourceResponse<DocumentCollection> response = await this.client.ReadDocumentCollectionAsync(docCollUri, requestOptions);
            Console.WriteLine(response.IndexTransformationProgress);
        }
    }
}