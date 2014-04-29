using System;

namespace TestData.CSharp
{
    /// <summary>
    /// Representation of a piece of data for the test
    /// </summary>
    public class DataPoint
    {
        public DateTime Timestamp;
        public object Data;
        public int Quality;

        /// <summary>
        /// Init the datapoint.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="data"></param>
        /// <param name="quality"></param>
        public DataPoint(DateTime timestamp, object data, int quality)
        {
            Timestamp = timestamp;
            Data = data;
            Quality = quality;
        }

        /// <summary>
        /// Just test data
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj != null)
            {
                if (obj is DataPoint)
                {
                    DataPoint other = obj as DataPoint;
                    if ((this.Data == null) && (other.Data == null))
                    {
                        result = true;
                    }
                    else if (this.Data != null)
                    {
                        result = this.Data.Equals(other.Data);
                    }
                }
            }

            return result;
        }

        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }
    }
}
