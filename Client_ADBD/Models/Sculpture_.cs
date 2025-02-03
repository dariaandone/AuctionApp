using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Client_ADBD.Models
{
    public class Sculpture_:IProduct
    {
        public string Material {  get; set; }
        public string Artist {  get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Depth { get; set; }
        AuctionAppEntities _dbContext;
        public Sculpture_()
        {
            _dbContext = new AuctionAppEntities();
        }


        public Sculpture_(int productId, string name, string description, DateTime invDate,decimal startPrice,decimal listPrice,string[] imagePaths,
            string material, string artist, decimal length, decimal width, decimal depth) : base(productId, name, description, invDate, startPrice, listPrice, imagePaths)
        {
            _dbContext = new AuctionAppEntities();
            Material = material;
            Artist = artist;
            Length = length;
            Width = width;
            Depth = depth;
        }

        public int GetSculptureMaterialId(string material)
        {
          

            var id = _dbContext.Sculpture_materials.Where(p => p.material == material).FirstOrDefault().id_sculpture_material;

            return id;
        }

        public void AddSculpturePost(int auctionNumber, decimal startPrice, decimal listPrice, DateTime creationTime, string[] imagePath,
          string productName, string description, DateTime inventoryDate, string artist, string material, decimal width, decimal length, decimal depth)
        {


            try
            {
                using (var transaction = new TransactionScope())
                {
                    var newProduct = new Product
                    {
                        name = productName,
                        description = description,
                        inventory_date = inventoryDate
                    };

                    _dbContext.Products.Add(newProduct);
                    _dbContext.SaveChanges();

                    int productId = newProduct.id_product;

                    int idMaterial = GetSculptureMaterialId(material);



                    var newSculpture = new Sculpture
                    {
                        id_product = productId,
                        id_sculpture_material = idMaterial,
                        artist = artist,
                        length = length,
                        depth = depth,
                        width = width
                    };

                    _dbContext.Sculptures.Add(newSculpture);
                    _dbContext.SaveChanges();

                    int idAuction = (new Auction_()).GetAuctionIdByNumber(auctionNumber);
                    int lotNumber = (new Post_()).GetNextLotNumber(idAuction);

                    var newPost = new Post
                    {
                        id_product = productId,
                        id_status = 2,
                        id_auction = idAuction,
                        start_price = startPrice,
                        list_price = listPrice,
                        created_at = creationTime,
                        lot = lotNumber,
                    };

                    _dbContext.Posts.Add(newPost);
                    _dbContext.SaveChanges();


                    var newImage = new Product_image
                    {
                        id_product = productId,
                        image_path = imagePath[0]
                    };

                    _dbContext.Product_images.Add(newImage);
                    _dbContext.SaveChanges();

                    if (!string.IsNullOrEmpty(imagePath[1]))
                    {
                        var newImage1 = new Product_image
                        {
                            id_product = productId,
                            image_path = imagePath[1]
                        };

                        _dbContext.Product_images.Add(newImage1);
                        _dbContext.SaveChanges();
                    }

                    if (!string.IsNullOrEmpty(imagePath[2]))
                    {
                        var newImage2 = new Product_image
                        {
                            id_product = productId,
                            image_path = imagePath[2]
                        };

                        _dbContext.Product_images.Add(newImage2);
                        _dbContext.SaveChanges();
                    }

                    transaction.Complete();


                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Eroare la adăugarea sculpturii: {ex.Message}");
            }
        }

        public void UpdateSculpturePostDetails(int productId, string artist, decimal length, decimal width, decimal depth, string material)
        {
            var sculpture = _dbContext.Sculptures.FirstOrDefault(s => s.id_product == productId);
            if (sculpture.artist != artist)
            {
                sculpture.artist = artist;
            }

            if (sculpture.length != length)
            {
                sculpture.length = length;
            }

            if (sculpture.width != width)
            {
                sculpture.width = width;
            }

            if (sculpture.depth != depth)
            {
                sculpture.depth = depth;
            }

            int materialId = GetSculptureMaterialId(material);

            if (materialId != sculpture.id_sculpture_material)
            {
                sculpture.id_sculpture_material = materialId;
            }

            _dbContext.SaveChanges();

        }
    }

}
