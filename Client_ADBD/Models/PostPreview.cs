using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_ADBD.Models
{
    internal class PostPreview
    {
        public int postId {  get; set; }    
        public string imagePath {  get; set; }
        public string postName {  get; set; }
        public string artistName {  get; set; }
        public decimal startPrice {  get; set; }
        public string postStatus {  get; set; }

        AuctionAppEntities _dbContext;
        public PostPreview()
        {
            _dbContext = new AuctionAppEntities();
        }

        public List<PostPreview> GetPostPreview(int auctionNumber, string sortType = "default", string postStatus = "default")
        {
            //_dbContext = new AuctionAppEntities();
            //var auctionId = (new Auction_()).GetAuctionIdByNumber(auctionNumber);
            //var auctionIdType =(new Auction_()).GetAuctionIdType(auctionId);
            //var auctionType =(new Auction_()).GetAuctionType(auctionIdType);


            //var query = from post in _dbContext.Posts
            //            join pr in _dbContext.Products on post.id_product equals pr.id_product
            //            join ps in _dbContext.Post_status on post.id_status equals ps.id_status
            //            join a in _dbContext.Auctions on post.id_auction equals a.id_auction
            //            where post.id_auction == auctionId
            //            select new PostPreview
            //            {
            //                postId = post.id_post,
            //                imagePath = (new IProduct()).GetFirstProductImagePath(pr.id_product),
            //                postName = pr.name,
            //                artistName = (new User_()).GetManufacturerName(auctionType, pr.id_product),
            //                startPrice = post.start_price,
            //                postStatus = ps.status_name
            //            };

            //if (postStatus != "default")
            //{
            //    query = query.Where(post => post.postStatus.ToLower() == postStatus.ToLower());
            //}

            //switch (sortType)
            //{
            //    case "PREȚ, ASCENDENT":
            //        query = query.OrderBy(post => post.startPrice);

            //        break;
            //    case "PREȚ, DESCENDENT":
            //        query = query.OrderByDescending(post => post.startPrice);
            //        break;
            //    default:
            //        break;

            //}

            //return query.ToList();


            // Obțineți ID-ul licitației și tipul acesteia
            var auctionId = (new Auction_()).GetAuctionIdByNumber(auctionNumber);
            var auctionIdType = (new Auction_()).GetAuctionIdType(auctionId);
            var auctionType = (new Auction_()).GetAuctionType(auctionIdType);

            // Interogare pentru obținerea posturilor
            var query = (from post in _dbContext.Posts
                         join pr in _dbContext.Products on post.id_product equals pr.id_product
                         join ps in _dbContext.Post_status on post.id_status equals ps.id_status
                         join a in _dbContext.Auctions on post.id_auction equals a.id_auction
                         where post.id_auction == auctionId
                         select new
                         {
                             postId = post.id_post,
                             productId = pr.id_product,
                             postName = pr.name,
                             startPrice = post.start_price,
                             postStatus = ps.status_name
                         })
                         .AsEnumerable() // Mută rezultatele în memorie
                         .Select(post => new PostPreview
                         {
                             postId = post.postId,
                             imagePath = (new IProduct()).GetFirstProductImagePath(post.productId), // Metoda C# va fi folosită aici
                             postName = post.postName,
                             artistName = (new User_()).GetManufacturerName(auctionType, post.productId), // Metoda C# va fi folosită aici
                             startPrice = post.startPrice,
                             postStatus = post.postStatus
                         });

            // Aplicăm filtrarea pentru `postStatus` (dacă este necesar)
            if (postStatus != "default")
            {
                query = query.Where(post => post.postStatus.ToLower() == postStatus.ToLower());
            }

            // Aplicăm sortarea pe baza `sortType`
            switch (sortType)
            {
                case "PREȚ, ASCENDENT":
                    query = query.OrderBy(post => post.startPrice);
                    break;
                case "PREȚ, DESCENDENT":
                    query = query.OrderByDescending(post => post.startPrice);
                    break;
                default:
                    break;
            }

            // Returnează lista finală
            return query.ToList();

        }
    }


}
