using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;


namespace Server_MoneyChange
{
    class CurrencyReciever
    {
        //hashtables for the current rates
        public static Hashtable kursKupuva;
        public static Hashtable kursProdava;
       
        private CurrencyReciever(Hashtable kupuva, Hashtable prodava)
        {
            kursKupuva = kupuva;
            kursProdava = prodava;
        }
        public static CurrencyReciever GetCurrencies()
        {
            //local hashtables
            Hashtable kupuva = new Hashtable();
            Hashtable prodava = new Hashtable();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://www.fibank.bg/bg/valutni-kursove");

            //scraping from the internet the buy rates
            #region kursKupuva
            HtmlNodeCollection curr = doc.DocumentNode.SelectNodes("/html/body/div[1]/main/section[1]/div/div[2]/div/table/tbody/tr[6]/td[5]");
            //using the xpath for each of the six currencies
            foreach (HtmlNode node in curr)
            {
                kupuva.Add("eur", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[13]/td[5]");
            foreach (HtmlNode node in curr)
            {
                kupuva.Add("usd", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[7]/td[5]");
            foreach (HtmlNode node in curr)
            {
                
                kupuva.Add("gbp", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[3]/td[5]");
            foreach (HtmlNode node in curr)
            {
                
                kupuva.Add("chf", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[1]/td[5]");
            foreach (HtmlNode node in curr)
            {
                kupuva.Add("aud", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[2]/td[5]");
            foreach (HtmlNode node in curr)
            {
                kupuva.Add("cad", node.InnerText);
            }
            #endregion

            //scraping from the internet the sell rates
            #region kursProdava
            //using xpath for each of the currencies
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[6]/td[6]");
            foreach (HtmlNode node in curr)
            {
                prodava.Add("eur", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[13]/td[6]");
            foreach (HtmlNode node in curr)
            {
                prodava.Add("usd", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[7]/td[6]");
            foreach (HtmlNode node in curr)
            {
                prodava.Add("gbp", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[3]/td[6]");
            foreach (HtmlNode node in curr)
            {
                prodava.Add("chf", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[1]/td[6]");
            foreach (HtmlNode node in curr)
            {
                prodava.Add("aud", node.InnerText);
            }
            curr = doc.DocumentNode.SelectNodes("/html/body/div/main/section[1]/div/div[2]/div/table/tbody/tr[2]/td[6]");
            foreach (HtmlNode node in curr)
            {
                prodava.Add("cad", node.InnerText);
            }
            #endregion

            return new CurrencyReciever(kupuva, prodava);
        }
    }
}
