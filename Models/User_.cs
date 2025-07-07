using Client_ADBD.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Client_ADBD.Models
{
    public class User_
    {
        public int _id { get; set; }

        public string _firstName { get; set; }
        public string _lastName { get; set; }
        public string _username { get; set; }
        public string _email { get; set; }
        public DateTime? _dateCreated { get; set; }
        public DateTime? _lastLogin { get; set; }
        public decimal? _balance { get; set; }

        public bool _isAdmin { get; set; }
        public bool _isBidder { get; set; }
        public bool _isCustomer { get; set; }

        public string _role { get; set; }

        AuctionAppEntities _dbContext;
        public User_()
        {
             _dbContext = new AuctionAppEntities();
        }
        public User_(int id, string firstname, string lastname, string username, string email, DateTime? date_created, DateTime? last_login, decimal? balance)
        {
            _dbContext = new AuctionAppEntities();
            _id = id;
            _firstName = firstname;
            _lastName = lastname;
            _username = username;
            _email = email;
            _dateCreated = date_created;
            _lastLogin = last_login;
            _balance = balance;

            int idUsername = GetRoleIdByUsername(username);

            if (idUsername == 2)
            {
                _role = "ofertant";
                _isAdmin = false; _isBidder = true; _isCustomer = false;
            }
            else if (idUsername == 3)
            {
                _role = "client";
                _isAdmin = false; _isBidder = false; _isCustomer = true;
            }
            else if (idUsername == 1)
            {
                _role = "administrator";
                _isAdmin = true; _isBidder = false; _isCustomer = false;
            }
            else
            {
                _isAdmin = false; _isBidder = false; _isCustomer = true;
            }
        }

        public void UpdateUser(string username, string firstName, string lastName, string email, decimal balance)
        {
            try
            {
                // Găsește utilizatorul în baza de date
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.username == username);

                if (existingUser == null)
                {
                    throw new Exception("Utilizatorul nu a fost găsit!");
                }

                // Actualizează câmpurile utilizatorului
                existingUser.fisrt_name = firstName;
                existingUser.last_name = lastName;
                existingUser.email = email;
                existingUser.balance = balance;

                // Salvează modificările
                _dbContext.SaveChanges();
                CurrentUser.User._balance = balance;
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la actualizarea utilizatorului: {ex.Message}");
            }
        }

        public List<User_> GetUsers()
        {
            var u = _dbContext.Users
                      .Select(u => new
                      {
                          u.id_user,
                          u.fisrt_name,
                          u.last_name,
                          u.username,
                          u.email,
                          u.created_at,
                          u.last_login,
                          u.balance
                      })
                      .ToList();

            List<User_> users = _dbContext.Users
                      .Select(u => new
                      {
                          u.id_user,
                          u.fisrt_name,
                          u.last_name,
                          u.username,
                          u.email,
                          u.created_at,
                          u.last_login,
                          u.balance
                      })
                      .ToList()
                      .Select(u => new User_(
                          id: u.id_user,
                          firstname: u.fisrt_name,
                          lastname: u.last_name,
                          username: u.username,
                          email: u.email,
                          date_created: u.created_at,
                          last_login: u.last_login,
                          balance: u.balance
                      ))
                      .ToList();

            return users;
        }

        public bool ChangeUserPassword(string username, string oldPassword, string newPassword)
        {
            var user = (from u in _dbContext.Users
                        where u.username == username
                        select new { u.username, u.password, u.salt }).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            if (!Hash.VerifyPassword(oldPassword, user.password, user.salt))
            {
                return false;
            }

            if (Hash.VerifyPassword(newPassword, user.password, user.salt))
            {
                return false;
            }

            string newSalt = Hash.GenerateSalt();
            string newPasswordHash = Hash.HashPassword(newPassword, newSalt);


            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.username == username);
            if (userToUpdate != null)
            {
                userToUpdate.password = newPasswordHash;
                userToUpdate.salt = newSalt;

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public int GetRoleIdByUsername(string username)
        {
            var item = _dbContext.Users.FirstOrDefault(u => u.username == username);
            if (item == null)
                throw new Exception("Utilizatorul nu există!");

            var role = _dbContext.User_roles.FirstOrDefault(r => r.id_user == item.id_user);
            if (role == null)
                throw new Exception("Nu există rol pentru acest utilizator!");

            return (int)role.id_tip;
        }

        public void UpdateUserRoleToBidder(string username)
        {
            // Creează un nou context pentru operația curentă
            try
            {
                // Găsește utilizatorul în baza de date
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.username == username);

                if (existingUser == null)
                {
                    throw new Exception("Utilizatorul nu a fost găsit în baza de date.");
                }

                // Găsește rolul asociat utilizatorului
                var role = _dbContext.User_roles.FirstOrDefault(r => r.id_user == existingUser.id_user);

                if (role == null)
                {
                    throw new Exception("Nu a fost găsit un rol asociat acestui utilizator.");
                }

                // Schimbă tipul de rol al utilizatorului (2 = "Bidder")
                role.id_tip = 2;

                // Salvează modificările în baza de date
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Aruncă o excepție cu detalii suplimentare
                throw new Exception($"Eroare la actualizarea rolului utilizatorului: {ex.Message}");
            }
        }

        public string GetUsernameById(int user_id)
        {
            var username = _dbContext.Users.Where(u => u.id_user == user_id).FirstOrDefault().username;
            return username;
        }

        public int GetUserIdByUsername(string username)
        {
            var id = _dbContext.Users
                     .Where(u => u.username == username)
                     .Select(u => u.id_user)
                     .FirstOrDefault();

            return id;
        }

        public int GetIdByUsername(string username)
        {
            var item = _dbContext.Users.FirstOrDefault(u => u.username == username);
            if (item == null)
                throw new Exception("Utilizatorul nu există!");

            return item.id_user;
        }

        public bool IsUserAdmin(string username)
        {
            
            try
            {
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.username == username);

                if (existingUser == null)
                {
                    throw new Exception("Utilizatorul nu a fost găsit în baza de date.");
                }

                var role = _dbContext.User_roles.FirstOrDefault(r => r.id_user == existingUser.id_user);


                if (role == null)
                {
                    throw new Exception("Nu a fost găsit un rol asociat acestui utilizator.");
                }

                if (role.id_tip == 1)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

         public bool VerifyUserCredentials(string username, string password)
        {
           

            var user = (from u in _dbContext.Users
                        where u.username == username
                        select new { u.username, u.password, u.salt }).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            return Hash.VerifyPassword(password, user.password, user.salt);
        }

        public void AddUser(string firstName, string lastName, string username, string password, string email)
        {

            var salt = Hash.GenerateSalt();
            var hashPassword = Hash.HashPassword(password, salt);

            var newUser = new User
            {
                fisrt_name = firstName,
                last_name = lastName,
                username = username,
                salt = salt,
                password = hashPassword,
                email = email,
                created_at = DateTime.Now,
                last_login = null,
                balance = 0.00M
            };


            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();

            var role = new User_role
            {
                id_user = newUser.id_user,
                id_tip = 3,
                role_date = DateTime.Now
            };

            _dbContext.User_roles.Add(role);
            _dbContext.SaveChanges();

        }

        public int CountTotalUsers()
        {
            return _dbContext.Users.Count() - 1;
        }

        public int CountTotalBidders()
        {
            return _dbContext.User_roles.Count(ur => ur.id_tip == 2);
        }

        public int CountTotalClients()
        {
            return _dbContext.User_roles.Count(ur => ur.id_tip == 3);
        }

        public bool AdminChangeUserPassword(string username, string newPassword)
        {
            var user = (from u in _dbContext.Users
                        where u.username == username
                        select new { u.username, u.password, u.salt }).FirstOrDefault();

            if (user == null)
            {
                return false;
            }

            if (Hash.VerifyPassword(newPassword, user.password, user.salt))
            {
                return false;
            }

            string newSalt = Hash.GenerateSalt();
            string newPasswordHash = Hash.HashPassword(newPassword, newSalt);


            var userToUpdate = _dbContext.Users.FirstOrDefault(u => u.username == username);
            if (userToUpdate != null)
            {
                userToUpdate.password = newPasswordHash;
                userToUpdate.salt = newSalt;

                _dbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public void UpdateUserRoleToClient(string username)
        {
            try
            {
                var existingUser = _dbContext.Users.FirstOrDefault(u => u.username == username);

                if (existingUser == null)
                {
                    throw new Exception("Utilizatorul nu a fost găsit în baza de date.");
                }

                var role = _dbContext.User_roles.FirstOrDefault(r => r.id_user == existingUser.id_user);

                if (role == null)
                {
                    throw new Exception("Nu a fost găsit un rol asociat acestui utilizator.");
                }

                role.id_tip = 3;

                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception($"Eroare la actualizarea rolului utilizatorului: {ex.Message}");
            }
        }

        public bool DeleteUserById(int userId)
        {
            try
            {
                var rolesToDelete = _dbContext.User_roles.Where(ur => ur.id_user == userId);
                if (rolesToDelete.Any())
                {
                    _dbContext.User_roles.RemoveRange(rolesToDelete);
                }

                var userInDb = _dbContext.Users.FirstOrDefault(u => u.id_user == userId);
                if (userInDb != null)
                {
                    _dbContext.Users.Remove(userInDb);
                }

                _dbContext.SaveChanges();
                return true;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la ștergerea utilizatorului și a relațiilor: {ex.Message}");
            }

            return false;
        }

        public IQueryable<User> GetUsersQuery()
        {
            return _dbContext.Users.AsQueryable();
        }

        public List<User> getUserListSorted(string SelectedSortOption)
        {
            // Obține toți utilizatorii înainte de filtrare/sortare
            var query = (new User_()).GetUsersQuery();

            // Aplică sortarea și filtrarea pe baza opțiunii selectate
            switch (SelectedSortOption)
            {
                case "Alfabetic după Username":
                    query = query.OrderBy(u => u.username);
                    break;

                case "Alfabetic după Rol":
                    query = query.OrderBy(u => u.User_role.OrderBy(r => r.id_tip).FirstOrDefault().id_tip);
                    break;

                case "Afișează doar Administratori":
                    query = query.Where(u => u.User_role.Any(r => r.id_tip == 1)); // Rolul 1 = Admin
                    break;

                case "Afișează doar Clienți":
                    query = query.Where(u => u.User_role.Any(r => r.id_tip == 3)); // Rolul 3 = Client
                    break;

                case "Afișează doar Ofertanți":
                    query = query.Where(u => u.User_role.Any(r => r.id_tip == 2)); // Rolul 2 = Ofertant
                    break;

                default:
                    query = query.OrderBy(u => u.username); // Sortare implicită
                    break;
            }

            // Convertim rezultatele în lista utilizatorilor (sau listă goală dacă nu există rezultate)
            var usersList = query.ToList();

            return usersList;
        }

        //public List<User> getListUsers()
        //{
        //    //var list =  _dbContext.Users
        //    //                .Select(u => new
        //    //                {
        //    //                    Id = u.id_user,
        //    //                    FirstName = u.fisrt_name,
        //    //                    LastName = u.last_name,
        //    //                    Username = u.username,
        //    //                    Email = u.email,
        //    //                    Balance = u.balance
        //    //                })
        //    //                .ToList();

        //    return _dbContext.Users.ToList();
        //}

        public ObservableCollection<User_> LoadUsers()
        {
            var usersList = _dbContext.Users
                                        .Select(u => new
                                        {
                                            Id = u.id_user,
                                            FirstName = u.fisrt_name,
                                            LastName = u.last_name,
                                            Username = u.username,
                                            Email = u.email,
                                            Balance = u.balance
                                        })
                                        .ToList();

            var Users = new ObservableCollection<User_>(usersList.Select(u => new User_(
                    id: u.Id,
                    firstname: u.FirstName,
                    lastname: u.LastName,
                    username: u.Username,
                    email: u.Email,
                    date_created: DateTime.Now,
                    last_login: DateTime.Now,
                    balance: u.Balance
            )));

            return Users;
        }

        public string GetManufacturerName(string auction_type, int productId)
        {

            switch (auction_type)
            {
                case "Ceasuri":
                    return _dbContext.Watches.Where(p => p.id_product == productId).FirstOrDefault().manufacturer ?? string.Empty;
                case "Bijuterii":
                    return _dbContext.Jewelries.Where(p => p.id_product == productId).FirstOrDefault().brand ?? string.Empty;
                case "Carti":
                    return _dbContext.Books.Where(p => p.id_product == productId).FirstOrDefault().author ?? string.Empty;
                case "Sculpturi":
                    return _dbContext.Sculptures.Where(p => p.id_product == productId).FirstOrDefault().artist ?? string.Empty;
                case "Tablouri":
                    return _dbContext.Paintings.Where(p => p.id_produs == productId).FirstOrDefault().artist ?? string.Empty;
                default:
                    return string.Empty;

            }
        }
    }
}