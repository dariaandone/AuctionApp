using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ADBD.Models
{
    public class IProduct
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime InventoryDate { get; set; }
        public decimal startPrice { get; set; }
        public decimal listPrice { get; set; }
        public string[] imagePaths {  get; set; }

        AuctionAppEntities _dbContext;

        public IProduct()
        {
            _dbContext = new AuctionAppEntities();
        }

        public IProduct(int productID, string name, string description, DateTime inventoryDate, decimal startPrice, decimal listPrice, string[] imagePaths)
        {
            _dbContext = new AuctionAppEntities();
            ProductID = productID;
            Name = name;
            Description = description;
            InventoryDate = inventoryDate;
            this.startPrice = startPrice;
            this.listPrice = listPrice;
            this.imagePaths = imagePaths;
        }

     

        public string GetFirstProductImagePath(int product_id)
        {
           

            var path = _dbContext.Product_images.Where(p => p.id_product == product_id).FirstOrDefault().image_path;

            return path;
        }
    }

}
