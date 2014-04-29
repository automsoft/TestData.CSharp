using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RHDALLib;

namespace TestData.CSharp
{
    public class DatabaseHelpers
    {
        private readonly ISession _Session;

        public DatabaseHelpers(ISession session)
        {
            _Session = session;
        }

        /// <summary>
        /// Create the requested node and items.
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="itemNames"></param>
        public void CreateNodeAndItems(string nodeName, IEnumerable<string> itemNames)
        {
            if (_Session.ActiveTrans)
                throw new InvalidOperationException("Unable to create data, as transaction is already open.");
            _Session.BeginTrans(TransactionMode_e.TXN_UPDATE_E, false);
            {
                //Get/Add Node
                var node = _Session.GetNode(nodeName) ?? _Session.AddNode(nodeName, NodeType_e.NODE_DATA_ENTRY_E);
                //Create all the required items
                foreach (var item in itemNames.Select(itemName => node.GetItem(itemName) ?? node.AddItem(itemName)))
                    Console.WriteLine("Item " + item._FullyQualifiedName + " created.");
            }
            _Session.CommitTrans();
        }

        /// <summary>
        /// Store data in the database, using the specified node and item name.
        /// </summary>
        public void StoreDataInDatabase(string nodename, string itemName, IEnumerable<DataPoint> dataToStore)
        {
            if (_Session.ActiveTrans)
                throw new InvalidOperationException("Unable to create data, as transaction is already open.");
            _Session.BeginTrans(TransactionMode_e.TXN_UPDATE_E, false);
            {
                //Get Node
                var node = _Session.GetNode(nodename);
                //Get Item
                var item = node.GetItem(itemName);
                //Now, add the data as specified by the user
                var eventLogger = item.GetEventLogger();
                foreach (var entry in dataToStore)
                {
                    object ts = (object)entry.Timestamp;
                    eventLogger.InsertEvent(ref ts, ref entry.Data, entry.Quality);
                }
                eventLogger.FlushEventsToDisk();
            }
            _Session.CommitTrans();
        }


        /// <summary>
        /// Pull all the data out of a specified item.
        /// </summary>
        /// <returns></returns>
        public IList<DataPoint> RetrieveDataInDatabase(string nodeName, string itemName)
        {
            var returnValue = new List<DataPoint>();
            if (_Session.ActiveTrans)
                throw new InvalidOperationException("Unable to create data, as transaction is already open.");
            _Session.BeginTrans(TransactionMode_e.TXN_UPDATE_E, false);
            try
            {
                //Get Node
                var node = _Session.GetNode(nodeName);
                if (node == null)
                    throw new InvalidOperationException("Unable to retrieve node: " + nodeName);
                //Get/Add Item
                var item = node.GetItem(itemName);
                if (item == null)
                {
                    throw new InvalidOperationException("Unable to retrieve item: " + itemName);
                }
                //Now, add the data as specified by the user
                var eventItr = item.GetEventIterator();
                object startTime = DateTime.MinValue;
                eventItr.Open(ref startTime, false);
                object timestamp = null;
                object data = null;
                int quality = 0;
                //Read out all the data.
                while (eventItr.GetNext(ref timestamp, out data, out quality))
                {
                    DateTime ts = DateTime.FromFileTime((long)timestamp);
                    var dataEntry = new DataPoint(ts, data, quality);
                    returnValue.Add(dataEntry);
                }
            }
            finally
            {
                _Session.CommitTrans();
            }
            return returnValue;
        }
    }
}
