using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace Docller.Core.Models
{
    public enum SubscriberItemType
    {
        Company=0,
        User=1
    }

    public abstract class SubscriberItem
    {
        public string Id 
        { 
            get
            {
                char prefix = this.SubscriberType.ToString()[0];
                return string.Concat(prefix.ToString(CultureInfo.InvariantCulture), this.GetId());
            }
        }
        public string Text 
        {
            get { return this.GetText(); }
        }
        
        public abstract SubscriberItemType SubscriberType { get;  }
        public abstract bool IsStartsWith(string input);
        public abstract bool Contains(string input);
        protected abstract object GetId();
        public abstract void SetId(object id);
        protected abstract string GetText();

        protected object ExtractId(object id)
        {
            return id != null ? id.ToString().Remove(0, 1) : id;
        }

        protected bool StartsWith(string value, string search)
        {
            return value != null && value.Trim().StartsWith(search.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        protected bool Contains(string value, string search)
        {
            return value != null && value.Trim().IndexOf(search.Trim(), StringComparison.InvariantCultureIgnoreCase) >= 0;
        }


      
    }

    public class SubscriberItemFactory
    {
        public static SubscriberItem Create(string idValue)
        {
            if (!string.IsNullOrEmpty(idValue))
            {
                char prefix = idValue[0];
                SubscriberItem item = null;
                if (prefix.Equals(SubscriberItemType.Company.ToString()[0]))
                {
                    item = new SubscriberCompany();
                }
                else if (prefix.Equals(SubscriberItemType.User.ToString()[0]))
                {
                    item = new SubscriberUser();
                }

                if (item != null)
                {
                    item.SetId(idValue);
                    return item;
                }
            }
            return null;

        }
   
    }

    public class SubscriberUser : SubscriberItem
    {
        public SubscriberUser()
        {
            
        }

        public SubscriberUser(User user)
        {
            this.UserId = user.UserId;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.Email = user.Email;
        }
        
        [JsonIgnore]
        public string FirstName { get; set; }
        [JsonIgnore]
        public string LastName { get; set; }

        [JsonIgnore]
        public int UserId { get; set; }

        [JsonIgnore]
        public string DisplayName 
        {
            get
            {
                if (!string.IsNullOrEmpty(this.LastName) || !string.IsNullOrEmpty(LastName))
                {
                    return string.Format(CultureInfo.InvariantCulture, "{0} {1}", this.FirstName, this.LastName).Trim();
                }
                return null;
            }
        }

        [JsonIgnore]
        public string Email { get; set; }

        public override SubscriberItemType SubscriberType
        {
            get { return SubscriberItemType.User;}
        }

        public override bool IsStartsWith(string input)
        {
            return !string.IsNullOrEmpty(DisplayName)
                       ? (StartsWith(FirstName, input) || StartsWith(LastName, input))
                       : StartsWith(Email, input);
        }

        public override bool Contains(string input)
        {
            return !string.IsNullOrEmpty(DisplayName)
                       ? (Contains(FirstName, input) || Contains(LastName, input))
                       : Contains(Email, input);
        }

        protected override object GetId()
        {
            return this.UserId;
        }

        public override void SetId(object id)
        {
            id = ExtractId(id);
            int userId;
            if (id != null && Int32.TryParse(id.ToString(), out userId))
            {
                this.UserId = userId;
            }
        }

        protected override string GetText()
        {
            return !string.IsNullOrEmpty(this.DisplayName) ? this.DisplayName : this.Email;
        }


    }

    public class SubscriberCompany:SubscriberItem
    {

        public SubscriberCompany()
        {
            
        }

        public SubscriberCompany(Company company)
        {
            this.CompanyId = company.CompanyId;
            this.CompanyName = company.CompanyName;
            this.CompanyAlias = company.CompanyAlias;
        }

        [JsonIgnore]
        public string CompanyName { get; set; }
        [JsonIgnore]
        public long CompanyId { get; set; }
        [JsonIgnore]
        public string CompanyAlias { get; set; }

        public override SubscriberItemType SubscriberType
        {
            get {   return SubscriberItemType.Company;}
        }

        public override bool IsStartsWith(string input)
        {
            return StartsWith(CompanyName, input);

        }

        public override bool Contains(string input)
        {
            return Contains(CompanyName, input);
        }

        protected override object GetId()
        {
            return this.CompanyId;
        }

        public override void SetId(object id)
        {
            id = ExtractId(id);
            long companyId;
            if (id != null && long.TryParse(id.ToString(), out companyId))
            {
                this.CompanyId = companyId;
            }
        }

        protected override string GetText()
        {
            return this.CompanyName;
        }

        public static SubscriberItem CreateFrom(Company company)
        {
            return new SubscriberCompany()
                {
            
                };
        }
    }

    
    public class SubscriberItemCollection:List<SubscriberItem>
    {
        public SubscriberItemCollection(IEnumerable<TransmittalUser> users)
        {
            this.Init(users);
        }
        public SubscriberItemCollection():base()
        {
            
        }

        public SubscriberItemCollection(string values)
        {
            this.Populate(values);
        }

        public SubscriberItemCollection(IEnumerable<SubscriberItem> items) : base(items)
        {
            
        }

        public SubscriberItemCollection(int capacity):base(capacity){}

        public void Populate(string values)
        {
            if (!string.IsNullOrEmpty(values))
            {
                string[] sValues = values.Split(",".ToArray(), StringSplitOptions.RemoveEmptyEntries);

                if (sValues.Length > 0)
                {
                    this.AddRange(sValues.Select(sValue => SubscriberItemFactory.Create(sValue)));
                }
            }

        }

        public string ToTextString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                stringBuilder.Append(this[i].Text);
                if (i < this.Count - 1)
                {
                    stringBuilder.Append(",");
                }
            }
            return stringBuilder.ToString();
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < this.Count; i++)
            {
                stringBuilder.Append(this[i].Id);
                if (i < this.Count-1)
                {
                    stringBuilder.Append(",");
                }
            }
            return stringBuilder.ToString();
        }

        private void Init(IEnumerable<TransmittalUser> users)
        {
            foreach (TransmittalUser transmittalUser in users)
            {
                this.Add(new SubscriberUser()
                    {
                        Email = transmittalUser.Email,
                        UserId = transmittalUser.UserId,
                        FirstName = transmittalUser.FirstName,
                        LastName = transmittalUser.LastName
                    });
            }
        }
    }

    

}
