// ref : ..\output\raptordb.dll
// ref : ..\output\raptordb.common.dll
// ref : ..\faker.dll
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RaptorDB;
using RaptorDB.Common;

namespace rdbtest
{

    #region [  entities  ]
    public class LineItem
    {
        public decimal QTY { get; set; }
        public string Product { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }

    public class SalesInvoice
    {
        public SalesInvoice()
        {
        }

        public Guid ID { get; set; } = Guid.NewGuid();
        public string CustomerName { get; set; }
        public string NoCase { get; set; }
        public string Address { get; set; }
        public List<LineItem> Items { get; set; } = new List<LineItem>();
        public DateTime Date { get; set; }
        public int Serial { get; set; }
        public byte Status { get; set; }
        public bool Approved { get; set; }
    }
    #endregion

    #region [  view definition  ]
    public class SalesInvoiceViewRowSchema : RDBSchema
    {
        public string CustomerName;
        public string NoCase;
        public DateTime Date;
        public string Address;
        public int Serial;
        public byte Status;
        public bool? Approved;
    }

    [RegisterView]
    public class SalesInvoiceView : View<SalesInvoice>
    {
        public SalesInvoiceView()
        {
            this.Name = "SalesInvoice";
            this.Description = "A primary view for SalesInvoices";
            this.isPrimaryList = true;
            this.isActive = true;
            this.BackgroundIndexing = true;
            this.Version = 3;

            this.Schema = typeof(SalesInvoiceViewRowSchema);
            this.FullTextColumns.Add("CustomerName");
            this.FullTextColumns.Add("Address");


            this.Mapper = (api, docid, doc) =>
            {
                //if (doc.Status == 0)
                //    return;
                
                api.EmitObject(docid, doc);
            };
        }
    }
    #endregion

    class Program
    {
        private const int   NumLineItems       = 5;
        private const int   NumOfInvoices      = 100_000;
        private static bool ShareInvoiceObject = false;

        private static SalesInvoice sharedInvoice = null;

        static RaptorDB.RaptorDB rdb; // 1 instance

        static void Main(string[] args)
        {
            rdb = RaptorDB.RaptorDB.Open("data"); // a "data" folder beside the executable
            RaptorDB.Global.RequirePrimaryView = false;

            Console.WriteLine("Registering views..");
            rdb.RegisterView(new SalesInvoiceView());

            DoWork();

            Console.WriteLine("press any key...");
            Console.ReadKey();
            Console.WriteLine("\r\nShutting down...");
            rdb.Shutdown(); // explicit shutdown
        }

        static void DoWork()
        {
            long c = rdb.DocumentCount();
            if (c > 0) // not the first time running
            {
                Stopwatch sw = Stopwatch.StartNew();
                var result = rdb.Query<SalesInvoiceViewRowSchema>(x => x.Serial < 100);
                sw.Stop();

                // show the rows
                Console.WriteLine(fastJSON.JSON.ToNiceJSON(result.Rows, new fastJSON.JSONParameters { UseExtensions = false, UseFastGuid = false }));
                // show the count
                Console.WriteLine($"Query result count = {result.Count:N0} took {sw.Elapsed}");
                return;
            }

            Console.Write($"Inserting {NumOfInvoices} documents...");

            int count = NumOfInvoices;
            SalesInvoice inv;
            for (int i = 0; i < count; i++)
            {
                inv = CreateInvoice(i);

                // save here
                rdb.Save(inv.ID, inv);
            }

            Console.WriteLine("done.");
        }

        static SalesInvoice CreateInvoice(int counter)
        {
            // new invoice
            SalesInvoice inv;
            if (ShareInvoiceObject)
            {
                sharedInvoice = sharedInvoice ?? NewInvoice();
                inv = sharedInvoice;
            }
            else
                inv = NewInvoice();

            inv.ID           = Guid.NewGuid();
            inv.Date         = Faker.DateTimeFaker.BirthDay();
            inv.Serial       = counter % 10000;
            inv.CustomerName = Faker.NameFaker.Name();
            inv.NoCase       = "Me " + counter % 10;
            inv.Status       = (byte)(counter % 4);
            inv.Address      = Faker.LocationFaker.Street();
            inv.Approved     = counter % 100 == 0 ? true : false;

            return inv;
        }

        private static SalesInvoice NewInvoice()
        {
            var inv = new SalesInvoice();
            // new line items
            inv.Items = new List<LineItem>();
            for (int k = 0; k < NumLineItems; k++)
                inv.Items.Add(new LineItem() {
                    Product  = "prod " + k,
                    Discount = 0,
                    Price    = 10 + k,
                    QTY      = 1 + k
                });

            return inv;
        }
    }
}