using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;

namespace ENSEKExercise.Models
{
	public class MeterReading
	{
		[Index(0)]
		public int AccountId { get; set; }
		[Index(1)]
		public string MeterReadingDateTime { get; set; }
		[Index(2)]
		public int MeterReadValue { get; set; }

		// the overrides below make sure that the custom class can't have duplicates in the list
		// otherwise Distinct on the list doesn't work
		public override bool Equals(object obj)
		{
			var item = obj as MeterReading;

			if (item != null)
			{
				if (this.AccountId == item.AccountId && this.MeterReadingDateTime == item.MeterReadingDateTime && this.MeterReadValue == item.MeterReadValue)
				{
					return true;
				}
			}

			return false;
		}

		public override int GetHashCode()
		{
			return this.AccountId.GetHashCode();
		}


		public string ValidateCSVFile(HttpPostedFile postedFile)
		{
			List<MeterReading> readingList = new List<MeterReading>();
			int unsuccessfulCount = 0;

			using (var reader = new StreamReader(postedFile.InputStream))
			{
				using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
				{
					csv.Read();
					csv.ReadHeader();
					while (csv.Read())
					{
						try
						{
							var record = csv.GetRecord<MeterReading>();
							DateTime hellp = DateTime.Parse(record.MeterReadingDateTime);
							if (ValidateRecord(record))
							{
								readingList.Add(record);
							}
							else
							{
								unsuccessfulCount++;
							}
						}
						catch (CsvHelperException exception)
						{
							//add to unsuccessful records
							unsuccessfulCount++;
						}
					}
				}

			}

			//trim duplicates
			int duplicateCount = (readingList.Count() - readingList.Distinct().Count());
			unsuccessfulCount += duplicateCount;
			readingList = readingList.Distinct().ToList();


			using (var ensekDB = new EnsekDB.Entities())
			{
				foreach (var m in readingList)
				{
					var meterReading = new EnsekDB.MeterReading()
					{
						AccountID = m.AccountId,
						MeterReadingDateTime = DateTime.Parse(m.MeterReadingDateTime),
						Value = m.MeterReadValue.ToString("00000")
					};
					ensekDB.MeterReadings.Add(meterReading);
				}
				ensekDB.SaveChanges();
			}

			return ("There were " + readingList.Count() + " successful entries and " + unsuccessfulCount + " unsuccessful entries.");
		}

		private bool ValidateRecord (MeterReading record)
		{
			bool accountExists = checkAccountExists(record.AccountId);
			bool fiveDigitNumber = CheckReadingValueFormat(record.MeterReadValue);
			if (accountExists && fiveDigitNumber)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private bool checkAccountExists (int accountID)
		{
			using (var ensekDB = new EnsekDB.Entities())
			{
				var account = ensekDB.Accounts.FirstOrDefault(u => u.AccountID == accountID);

				if (account == null)
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckReadingValueFormat(int readingValue)
		{
			if (readingValue < 0)
			{
				return false;
			}

			//get digits of value
			int count = 0;
			while (readingValue > 0)
			{
				readingValue /= 10;
				count++;
			}
			if (count > 5)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		//public bool checkAccountReadingDate ()
		//{

		//}
	}


}