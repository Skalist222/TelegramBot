using aaa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

internal class UserWorker
{
    public class UserI
    {
        long id;
        string name;
        string firstname;
        string lastName;

     
        public bool AddedMem { get; set; }

        public long Id { get { return id; } }
        public string Name { get { return name; } }
        public string Firstname { get { return firstname; } }
        public string LastName { get { return lastName; } }
        

        public UserI(long id, string name, string firstname, string lastName)
        {
            this.id = id;
            this.name = name;
            this.firstname = firstname;
            this.lastName = lastName;
            AddedMem = false;

        }
    }
    public class UserList : List<UserI>
    {
        public UserList() { }
        public UserList(string pathFile)
        {
            string[] userStrings = System.IO.File.ReadAllLines(pathFile);
            foreach (string userString in userStrings)
            {
                string[] split = userString.Split('|');
                UserI user = new UserI(Int64.Parse(split[0]), split[1], split[2], split[3]);
            }
        }

        public UserI? SearchUser(long id)
        {
            foreach (UserI u in this)
            {
                if (u.Id == id) return u;
            }
            return null;
        }
        public bool SaveUsersInFile(string pathFile,UserList users)
        {
            bool allGood = true;
            return allGood;
        }
        public bool SaveUserInFile(string pathFile, UserI user)
        {
            try {
                string userString = user.Id+"|"+user.LastName+"|"+user.Firstname+"|"+user.Name+ Environment.NewLine;
                if (SearchUser(user.Id) is null)
                {
                    Add(user);
                    System.IO.File.AppendAllText(pathFile, userString);
                    return true;
                }
                else return false;
               
            }
            catch (Exception e)
            {
                return false;
            } 
        }
    }

    
}
    

