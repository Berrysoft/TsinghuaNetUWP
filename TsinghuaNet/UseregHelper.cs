﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace TsinghuaNet
{
    /// <summary>
    /// A simple structure represents the status of a connection.
    /// </summary>
    public class NetUser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetUser"/> class.
        /// </summary>
        /// <param name="address">IP address the connection was allocated.</param>
        /// <param name="loginTime">Online time used this time of the connection.</param>
        /// <param name="client">The client used by this connection.</param>
        public NetUser(IPAddress address, DateTime loginTime, string client)
        {
            Address = address;
            LoginTime = loginTime;
            Client = client;
        }
        /// <summary>
        /// IP address the connection was allocated.
        /// </summary>
        public IPAddress Address { get; }
        /// Online time used this time of the connection.
        /// </summary>
        public DateTime LoginTime { get; }
        /// <summary>
        /// The client used by this connection. It may be "Unknown" through <see cref="NetHelper"/>, and "Windows NT", "Windows 8", "Windows 7" or "Unknown" through <see cref="AuthHelper"/>.
        /// </summary>
        public string Client { get; }

        public static bool operator ==(NetUser u1, NetUser u2)
        {
            if (u1 is null || u2 is null)
                return object.ReferenceEquals(u1, u2);
            else
                return u1.Address.Equals(u2.Address) && u1.LoginTime == u2.LoginTime && u1.Client == u2.Client;
        }

        public static bool operator !=(NetUser u1, NetUser u2) => !(u1 == u2);

        /// <summary>
        /// Determines whether the two <see cref="NetUser"/> are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns><see langword="true"/> if they're equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is NetUser other)
            {
                return this == other;
            }
            return false;
        }
        /// <summary>
        /// Returns the hash value of this object.
        /// </summary>
        /// <returns>The hash value.</returns>
        public override int GetHashCode()
        {
            return (Address?.GetHashCode() ?? 0) ^ LoginTime.GetHashCode() ^ (Client?.GetHashCode() ?? 0);
        }
    }

    /// <summary>
    /// Flux detail.
    /// </summary>
    public class NetDetail
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NetDetail"/> class.
        /// </summary>
        /// <param name="login">The time logging in.</param>
        /// <param name="logout">The time logging out.</param>
        /// <param name="flux">The flux has been used.</param>
        public NetDetail(DateTime login, DateTime logout, ByteSize flux)
        {
            LoginTime = login;
            LogoutTime = logout;
            Flux = flux;
        }
        /// <summary>
        /// The time logging in.
        /// </summary>
        public DateTime LoginTime { get; }
        /// <summary>
        /// The time logging out.
        /// </summary>
        public DateTime LogoutTime { get; }
        /// <summary>
        /// The flux has been used.
        /// </summary>
        public ByteSize Flux { get; }

        public static bool operator ==(NetDetail d1, NetDetail d2)
        {
            if (d1 is null || d2 is null)
                return object.ReferenceEquals(d1, d2);
            else
                return d1.LoginTime == d2.LoginTime && d1.LogoutTime == d2.LogoutTime && d1.Flux == d2.Flux;
        }

        public static bool operator !=(NetDetail d1, NetDetail d2) => !(d1 == d2);

        /// <summary>
        /// Determines whether the two <see cref="NetDetail"/> are equal.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns><see langword="true"/> if they're equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is NetDetail other)
            {
                return this == other;
            }
            return false;
        }
        /// <summary>
        /// Returns the hash value of this object.
        /// </summary>
        /// <returns>The hash value.</returns>
        public override int GetHashCode()
        {
            return LoginTime.GetHashCode() ^ LogoutTime.GetHashCode() ^ Flux.GetHashCode();
        }
    }

    public enum NetDetailOrder
    {
        LoginTime,
        LogoutTime,
        Flux
    }

    /// <summary>
    /// Exposes methods to login, logout, get connection information and drop connections from https://usereg.tsinghua.edu.cn/
    /// </summary>
    public class UseregHelper : NetHelperBase, ILog
    {
        private const string LogUri = "http://usereg.tsinghua.edu.cn/do.php";
        private const string InfoUri = "http://usereg.tsinghua.edu.cn/online_user_ipv4.php";
        private const string DetailUri = "http://usereg.tsinghua.edu.cn/user_detail_list.php?action=query&desc={6}&order={5}&start_time={0}-{1}-01&end_time={0}-{1}-{2}&page={3}&offset={4}";
        private const string LogoutData = "action=logout";
        private const string DropData = "action=drop&user_ip={0}";
        /// <summary>
        /// Initializes a new instance of the <see cref="UseregHelper"/> class.
        /// </summary>
        /// <param name="username">The username to login.</param>
        /// <param name="password">The password to login.</param>
        /// <param name="client">A user-specified instance of <see cref="HttpClient"/>.</param>
        public UseregHelper(string username, string password, HttpClient client)
            : base(username, password, client)
        { }
        /// <summary>
        /// Login to the website.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public async Task<LogResponse> LoginAsync() => LogResponse.ParseFromUsereg(await PostAsync(LogUri, GetLoginData()));
        /// <summary>
        /// Logout from the website.
        /// </summary>
        /// <returns>The response of the website.</returns>
        public async Task<LogResponse> LogoutAsync() => LogResponse.ParseFromUsereg(await PostAsync(LogUri, LogoutData));
        /// <summary>
        /// Drop the IP address from network.
        /// </summary>
        /// <param name="ip">The IP address to be dropped.</param>
        /// <returns>The response of the website.</returns>
        public async Task<LogResponse> LogoutAsync(IPAddress ip) => LogResponse.ParseFromUsereg(await PostAsync(InfoUri, string.Format(DropData, ip.ToString())));

        /// <summary>
        /// Get login data with username and password.
        /// </summary>
        /// <returns>A dictionary contains the data.</returns>
        private Dictionary<string, string> GetLoginData()
        {
            return new Dictionary<string, string>
            {
                ["action"] = "login",
                ["user_login_name"] = Username,
                ["user_password"] = CryptographyHelper.GetMD5(Password)
            };
        }

        /// <summary>
        /// Get all connections of this user.
        /// </summary>
        /// <returns><see cref="IEnumerable{NetUser}"/></returns>
        public async Task<IEnumerable<NetUser>> GetUsersAsync()
        {
            string userhtml = await GetAsync(InfoUri);
            var doc = new HtmlDocument();
            doc.LoadHtml(userhtml);
            return from tr in doc.DocumentNode.Element("html").Element("body").Element("table").Element("tr").Elements("td").Last().Elements("table").ElementAt(1).Elements("tr").Skip(1)
                   let tds = (from td in tr.Elements("td").Skip(1)
                              select td.FirstChild?.InnerText).ToArray()
                   select new NetUser(
                       IPAddress.Parse(tds[0]),
                       DateTime.ParseExact(tds[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                       tds[10]);
        }

        private readonly static Dictionary<NetDetailOrder, string> OrderQueryMap = new Dictionary<NetDetailOrder, string>
        {
            [NetDetailOrder.LoginTime] = "user_login_time",
            [NetDetailOrder.LogoutTime] = "user_drop_time",
            [NetDetailOrder.Flux] = "user_in_bytes",
        };

        /// <summary>
        /// Get all details of this month.
        /// </summary>
        /// <returns><see cref="IEnumerable{NetDetail}"/></returns>
        public async Task<IEnumerable<NetDetail>> GetDetailsAsync(NetDetailOrder order, bool descending)
        {
            const int offset = 100;
            DateTime now = DateTime.Now;
            List<NetDetail> list = new List<NetDetail>();
            for (int i = 1; ; i++)
            {
                string detailhtml = await GetAsync(string.Format(DetailUri, now.Year, now.Month.ToString().PadLeft(2, '0'), now.Day, i, offset, OrderQueryMap[order], descending ? "DESC" : string.Empty));
                var doc = new HtmlDocument();
                doc.LoadHtml(detailhtml);
                int oldsize = list.Count;
                list.AddRange(
                    from tr in doc.DocumentNode.Element("html").Element("body").Element("table").Element("tr").Elements("td").Last().Elements("table").Last().Elements("tr").Skip(1)
                    let tds = (from td in tr.Elements("td").Skip(1)
                               select td.FirstChild?.InnerText).ToArray()
                    select new NetDetail(
                        DateTime.ParseExact(tds[1], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(tds[2], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
                        ByteSize.Parse(tds[4])));
                if (list.Count - oldsize < offset) break;
            }
            return list;
        }
    }
}