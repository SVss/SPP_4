using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using RssReader.Utils;

namespace RssReader.Model
{
    public class AppModel
    {
        public List<UserModel> UsersList { get; private set; } = new List<UserModel>();

        public UserModel AddUser(string name, int threadsCount)
        {
            var result = new UserModel(name, threadsCount);

            UsersList.Add(result);

            return result;
        }

        public void RemoveUser(UserModel user)
        {
            UsersList.Remove(user);
        }

        public void LoadUsersFromXml(XmlDocument document)
        {
            XmlNode root = document.FirstChild;
            if (root is XmlDeclaration)
            {
                root = root.NextSibling as XmlElement;
            }

            if ((root == null) || (root.Name != ConfigConsts.RootTag))
            {
                throw new BadXmlException();
            }

            foreach (XmlElement user in root.ChildNodes)
            {
                UsersList.Add(UserModel.FromXmlElement(user));
            }
        }

        public XmlDocument SaveUsersToXml()
        {
            XmlDocument result = new XmlDocument();

            XmlElement root = result.CreateElement(ConfigConsts.RootTag);
            foreach (UserModel user in UsersList)
            {
                root.AppendChild(user.ToXmlElement(result));
            }

            result.AppendChild(root);

            return result;
        }
    }
}
