using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace HtmlParser
{
  class Program
  {
    public static List<string> GetGroupList(string _pattern, string _text)
    {
      List<string> groupList = new List<string>();
      Regex r = new Regex(_pattern);
      Match m = r.Match(_text);
      int matchCount = 0;
      while (m.Success)
      {
        for (int i = 1; i <= 2; i++)
        {
          Group g = m.Groups[i];
          CaptureCollection cc = g.Captures;
          for (int j = 0; j < cc.Count; j++)
          {
            Capture c = cc[j];
            groupList.Add(c.ToString());
          }
        }
        m = m.NextMatch();
      }
      return groupList;
    }
    static void Main(string[] args)
    {
      var urlBase = "https://otzovik.com";
      var urlProduct = "reviews/laboratoriya_invitro_russia/";
      var pathToChanks = "//*[@id=\"content\"]/div/div/div/div/div[3]/div[1]/div[1]";
      var pathToLastPage = "//*[@id=\"content\"]/div/div/div/div/div[3]/div[1]/div[3]/a[12]";
      string patternUrls = @"content=""(https:\/\/otzovik\.com\/review_\d*\.html)"">";
      string patternLastPage =$@"\/(\d*)\/";
      
     
      List<string> urlReviewsList = new();
      List<string> chanksName = new();
      List<string> reviews = new();

      //string AllHtml =String.Empty;

      //var chanks = document.DocumentNode.SelectNodes(pathToChanks).Nodes();
      //foreach (var chank in chanks)
      //{
      //  AllHtml+=chank.InnerHtml;
      //}
      //urlReviewsList.AddRange(GetGroupList(patternUrls, AllHtml));

      //=======================
      // По всем страницам собираются html содержимое товаров 
      //=======================
      HtmlWeb web = new HtmlWeb();
      HtmlDocument document = web.Load(String.Concat(urlBase, "/", urlProduct));
      var lastPage = document.DocumentNode.SelectNodes(pathToLastPage).First().Attributes[1];
      var lastPageNumber = int.Parse(GetGroupList(patternLastPage, lastPage.Value).First());
      var currentUrlPage = String.Empty;
      var htmlFromAllProductPages = String.Empty;

      for (int i = 1; i <= lastPageNumber; i++) //для прохода по страницам
      {
        currentUrlPage = $"{urlBase}/{urlProduct}/{i}/";
        HtmlDocument page = web.Load(currentUrlPage);
        var reviewsChank = document.DocumentNode.SelectNodes(pathToChanks).Nodes();

        foreach (var chank in reviewsChank)
        {
          htmlFromAllProductPages += chank.InnerHtml;
        }
      }

      //=======================
      // Извлечение ссылок на отзывы
      //=======================

      urlReviewsList.AddRange(GetGroupList(patternUrls, htmlFromAllProductPages));

      //=======================
      // Проход по всем страницам в списке, извелечение тела отзыва
      //=======================
      var pathToReviewDesc = "//*[@id=\"content\"]/div/div/div/div/div[3]/div[1]/div[7]";
      foreach (var urlReview in urlReviewsList)
      {
        HtmlDocument reviewPage = web.Load(urlReview);
        var reviewDesc = document.DocumentNode.SelectNodes(pathToReviewDesc).First().InnerText;
        if (reviewDesc!= null) reviews.Add(reviewDesc);
      }

      //foreach (var block in blocks)
      //{
      //  urlsReview.Add(block.Attributes[1]);
      //}

      Console.WriteLine("======================");
    }

  }
}

