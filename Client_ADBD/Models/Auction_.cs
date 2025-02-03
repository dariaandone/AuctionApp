using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Client_ADBD.Models
{
    public enum AuctionStatus
    {
        Upcoming, 
        Ongoing, 
        Closed    
    }


    public class Auction_
    {
        public static readonly string DEFAULT_IMAGE_PATH = @"C:\Users\laris\Desktop\Client_ADBD\Client_ADBD\Views\photos\no-image_image.jpg";
        public int id { get; set; } 

        public int auctionNumber {  get; set; } 
        public string name { get; set; } 
        public DateTime startTime { get; set; } 
        public DateTime endTime { get; set; } 
        public string location { get; set; }
        public AuctionStatus status { get; set; }
        public string statusStr => StatusToString(status);
        public string imagePath { get; set; }
        public string description { get; set; }

        public string usernameOwner {  get; set; }  
        public string auctionType {  get; set; }

        AuctionAppEntities _dbContext;

        public Auction_() {
            _dbContext = new AuctionAppEntities();
        }
        public Auction_(Auction a)
        {
            _dbContext = new AuctionAppEntities();
            id =a.id_auction;
            name = a.name;
            startTime=a.start_time;
            endTime=a.end_time;
            location = a.location;
            imagePath=a.image_path;
            description = a.description;
            auctionNumber=a.auction_number;
            usernameOwner = (new User_()).GetUsernameById(a.id_user);
            auctionType = GetAuctionType(a.id_auction_type);
        }
       
        public static string StatusToString(AuctionStatus status)
        {
            switch (status)
            {
                case AuctionStatus.Upcoming:
                    return "Upcoming";
                case AuctionStatus.Ongoing:
                    return "Ongoing";
                case AuctionStatus.Closed:
                    return "Closed";
                default:
                    return "Unknown";
            }
        }

        public void AddAuction(string auctionName, string auctionType, DateTime startDateTime, DateTime endDateTime, string imagePath, string description, string location, string usernameUser)
        {
            int idType = GetAuctionIdType(auctionType);
            int idUser = (new User_()).GetUserIdByUsername(usernameUser);

            var newAuction = new Auction
            {
                name = auctionName,
                start_time = startDateTime,
                end_time = endDateTime,
                image_path = imagePath,
                location = location,
                description = description,
                id_auction_type = idType,
                auction_number = (new Auction_()).GetAuctionNumber() + 1,
                id_user = idUser
            };

            _dbContext.Auctions.Add(newAuction);
            _dbContext.SaveChanges();

        }

         public int GetAuctionIdType(int auction_id)
        {
       
            var id = _dbContext.Auctions.Where(p => p.id_auction == auction_id).FirstOrDefault().id_auction_type;

            return id;
        }

        public int GetAuctionNumber()
        {
 
            var id = _dbContext.Auctions
                    .Max(t => (int?)t.auction_number) ?? 0;

            return id;
        }

        public string GetAuctionType(int idAuctionType)
        {
            string type = _dbContext.Auction_types.Where(t => t.id_auction_type == idAuctionType).FirstOrDefault().type_name;
            return type;
        }

        int GetAuctionIdType(string type)
        {
            var idType = _dbContext.Auction_types
                          .Where(t => t.type_name == type)
                          .Select(t => t.id_auction_type)
                          .FirstOrDefault();

            return idType;
        }

        public Auction_ GetAuctionByName(string auctionName)
        {
            var auction = _dbContext.Auctions
                         .FirstOrDefault(a => a.name == auctionName);

            if (auction == null)
            {
                throw new InvalidOperationException($"Auction with name '{auctionName}' not found.");
            }

            return new Auction_(auction);
        }

        public int GetAuctionIdByNumber(int number)
        {
            var id = _dbContext.Auctions.Where(p => p.auction_number == number).FirstOrDefault().id_auction;

            return id;
        }

        public Auction_ GetAuctionByNumber(int auctionNumber)
        {

            var auction = _dbContext.Auctions
                         .FirstOrDefault(a => a.auction_number == auctionNumber);

            if (auction == null)
            {
                throw new InvalidOperationException($"Auction with name '{auctionNumber}' not found.");
            }

            return new Auction_(auction);

        }

        public  List<Auction_> GetAuctionsByUserId(int userId, string statusFilter = "default", string sortFilter = "default")
        {
            // Creează o instanță de DataContext
          
                var query = _dbContext.Auctions.Where(a => a.id_user == userId);

                // Maparea între obiectele Auction și Auction_
                var auctionList = query.Select(a => new Auction_
                {
                    id = a.id_auction,               // Mapare id
                    name = a.name,           // Mapare name
                    startTime = a.start_time, // Mapare start_time
                    endTime = a.end_time,    // Mapare end_time
                    location = a.location,   // Mapare location
                    imagePath = a.image_path, // Mapare imagePath
                    auctionNumber = a.auction_number // Mapare auctionNumber
                }).ToList(); // Execută și convertește la List<Auction_>

                return auctionList; // Returnează lista de Auction_
            
        }

         public bool DeleteAuction(int auctionNumber)
        {
           

            var auctionToDelete = _dbContext.Auctions.SingleOrDefault(a => a.auction_number == auctionNumber);

            if (auctionToDelete != null)
            {
                _dbContext.Auctions.Remove(auctionToDelete);
                _dbContext.SaveChanges();

                return false;
            }

            return true;
        }

        public List<Auction_> GetAuction(string auctionStatus, string filter)
        {

            var query = _dbContext.Auctions.AsQueryable();

            if (!string.Equals(auctionStatus, "default", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(auction =>
                   auctionStatus.Equals("Closed", StringComparison.OrdinalIgnoreCase)
                                ? auction.end_time <= DateTime.Now
                                : true);
            }

            if (string.Equals(filter, "Crescător", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderBy(auction => auction.start_time);
            }
            else if (string.Equals(filter, "Descrescător", StringComparison.OrdinalIgnoreCase))
            {
                query = query.OrderByDescending(auction => auction.start_time);
            }

            //var auctType = GetAuctionType(auction.id_auction_type);

            //// Mapare în DTO dacă `Auction_` este diferit de `Auction`
            //return query
            //    .Select(auction => new Auction_
            //    {
            //        id = auction.id_auction,
            //        name = auction.name,
            //        startTime = auction.start_time,
            //        endTime = auction.end_time,
            //        description = auction.description,
            //        location = auction.location,
            //        imagePath = auction.image_path,
            //        auctionNumber = auction.auction_number,
            //        auctionType = auctType

            //    })
            //    .ToList();

            return query
                    .AsEnumerable()  // Se mută datele în memorie înainte de `.Select()`
                    .Select(auction => new Auction_
                    {
                        id = auction.id_auction,
                        name = auction.name,
                        startTime = auction.start_time,
                        endTime = auction.end_time,
                        description = auction.description,
                        location = auction.location,
                        imagePath = auction.image_path,
                        auctionNumber = auction.auction_number,
                        auctionType = GetAuctionType(auction.id_auction_type) // Acum funcționează
                    })
                    .ToList();
        }

        public decimal GetTotalBidsForAuction(int nr)
        {
                // Găsește id-ul licitației
                var auction = _dbContext.Auctions
                    .FirstOrDefault(a => a.auction_number == nr);

                if (auction == null)
                    return 0; // Licitația nu există
                              // Găsește toate id-urile postărilor asociate licitației
                var postIds = _dbContext.Posts
                    .Where(p => p.id_auction == auction.id_auction)
                    .Select(p => p.id_post)
                    .ToList();

                var verificare = _dbContext.Bids.Where(b => postIds.Contains(b.id_post));
                if (!verificare.Any())  // Dacă nu există niciun element în verificare
                {
                    return 0;  // Dacă nu există intrări, returnăm 0
                }

                var total = _dbContext.Bids
                    .Where(b => postIds.Contains(b.id_post) && b.bid_price != null)  // Verificăm dacă bid_price nu este null
                    .Sum(b => b.bid_price);  // Calculăm suma doar pentru valori valide


                return total;
            
        }

         public int GetSoldItemsInAuction(int auctionNumber)
        {
          
                int soldItemsCount = 0;


                //  Obținem toate postările asociate licitației
                int auctionId = GetAuctionIdByNumber(auctionNumber);
                var posts = _dbContext.Posts
                    .Where(p => p.id_auction == auctionId)  // Filtrăm postările care sunt asociate cu licitația
                    .ToList();


                foreach (var post in posts)
                {
                    string nume = string.Empty;

                    // Apelăm funcția GetPostLastOffer pentru fiecare postare
                    var res =(new Post_()).GetPostLastOffer(post.id_post, ref nume);

                    // Dacă GetPostLastOffer returnează un rezultat valid (adică diferit de -1), considerăm postarea vândută
                    if (res != -1)
                    {
                        soldItemsCount++;  // Creștem contorul de postări vândute
                    }
                }

                return soldItemsCount;  // Returnăm numărul de postări vândute
           
        }

        public int GetTotalItemsInAuction(int auctionNumber)
        {
            
                // Găsește id-ul licitației
                var auction = _dbContext.Auctions
                    .FirstOrDefault(a => a.auction_number == auctionNumber);

                if (auction == null)
                    return 0; // Licitația nu există

                // Numără toate postările asociate licitației
                var totalItems = _dbContext.Posts
                    .Count(p => p.id_auction == auction.id_auction);

                return totalItems;
           
        }

        public double GetSoldPercentage(int auctionNumber)
        {
          
            // Găsește licitația asociată
            var auction = _dbContext.Auctions
                .FirstOrDefault(a => a.auction_number == auctionNumber);

            if (auction == null)
                return 0; // Licitația nu există, procentul este 0

            // Obține numărul total de articole
            var totalItems = GetTotalItemsInAuction(auctionNumber);

            if (totalItems == 0)
                return 0; // Dacă nu există articole, procentul este 0

            // Obține numărul articolelor vândute
            var soldItems = GetSoldItemsInAuction(auctionNumber);

            // Calculează procentul
            return (double)soldItems / totalItems * 100;
           
        }

        public void AddBid(int postId, int idUsesr, decimal bidPrice)
        {
            var newBid = new Bid
            {
                id_post = postId,
                id_user = idUsesr,
                bid_price = bidPrice,
                bid_date = DateTime.Now,
            };

            _dbContext.Bids.Add(newBid);
            _dbContext.SaveChanges();
        }

        public void UpdateAuction(Auction_ auctionToUpdate)
        {
            var auction = _dbContext.Auctions.SingleOrDefault(a => a.auction_number == auctionToUpdate.auctionNumber);

            if (auction != null)
            {
                if (auctionToUpdate.name != auction.name)
                    auction.name = auctionToUpdate.name;

                if (auctionToUpdate.startTime != auction.start_time)
                    auction.start_time = auctionToUpdate.startTime;

                if (auctionToUpdate.endTime != auction.end_time)
                    auction.end_time = auctionToUpdate.endTime;

                if (auctionToUpdate.imagePath != auction.image_path)
                    auction.image_path = auctionToUpdate.imagePath;

                if (auctionToUpdate.description != auction.description)
                    auction.description = auctionToUpdate.description;

                if (auctionToUpdate.location != auction.location)
                    auction.location = auctionToUpdate.location;

                _dbContext.SaveChanges();
            }

        }
    }
}
