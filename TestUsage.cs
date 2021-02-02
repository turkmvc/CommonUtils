using CommonUtils.Attributes;
using CommonUtils.Classes;
using CommonUtils.Extensions;
using CommonUtils.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace CommonUtilsTest
{
 
    public partial class TestUsage
    {
        private void StringSplitTest()
        {
            //Normal Split fonksiyonundan ek olarak ayarlar ve tek/çift tırnak içerisini ayırmama gibi özellikler barındırır.
            StringSplitter splitter = new StringSplitter("item1 => item2 => item3 = item4", "=", "=>");
            splitter.OnSplit = (handler) =>
            {
                //her metindeki boşlukları kırpıyoruz.
                handler.Text = handler.Text.Trim();
                //item2 ismindeki metni listeye dâhil ettirmiyoruz.
                if (handler.Text == "item2") handler.Cancel = true;
                //Mevcut metin * ile başlıyorsa ayrıştırma işlemini bu kısımda durdurur.
                if(handler.Text.StartsWith("*"))
                {
                    handler.Stop = true;
                }
            };
            //Çift Tırnak/Tek tırnak içerisindeki metinlerin ayraca dâhil edip etmeyeceği buradan ayarlanır.
            splitter.SplitQuoteOption = StringQuoteOption.SingleQuote;
            var result = splitter.Split();
            MessageBox.Show( string.Join("\r\n", result));


            //Ayrıca aşağıdaki gibide kullanılabilir.
            //using CommonUtils.Extensions; içermelidir.
            result = "item1 item2 item3".SplitEx(" ");
        }
        private void StringTokenizerTest()
        {
            //StringSplitter sınıfıda bu sınıfı kullanır.
            //C++ dilindeki strtok() fonksiyonuna benzerdir, verilen anahtara kadar olan metni dönderir, her çağrıldığında
            //gelen metinden sonraki kalan metin üzerinde arama yapar, bir nevi split ile aynı işlevi görür.
            //2 Türlü kullanımı mevcuttur, Extensions olarak kullanımın da, yalnızca string dönüş yapar,
            //fakat sınıf olarak kullanılması halinde StringTokenResult sınıfı olarak dönüş yapar.

            //1. Tür kullanımı;
            StringTokenizer stringTokenizer = new StringTokenizer("cw arge+macmillan*deneme");
            string[] ara = new string[] {" ", "+", "*"};
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < ara.Length; i++)
            {
                sb.AppendFormat("Aranan Token: {0}\r\n", ara[i]);
                StringTokenResult result = stringTokenizer.Tokenize(ara[i]);
                sb.AppendFormat("Dönen Metin: {0}\r\n", result.TokenText);
                sb.Append("-------------------------");
                sb.AppendLine();
                //Tokenin bulunup bulunmadığı
               // result.TokenFound 
                //Tokenin metindeki konumu(bulunmazsa -1 döner)
                //result.TokenIndex
                //Bulunan Token(Çoklu token aramaları için)
                //result.TokenKey
                
            }
            //Token içerisindeki metinde son kısma gelinip gelinmediği...
            if(!stringTokenizer.Finish)
            {
                sb.AppendFormat("Kalan Metin: {0}", stringTokenizer.GetRemainText());

            }
            MessageBox.Show(sb.ToString());


            //2. Tür kullanımı(extension olarak, string dönderir.
            sb.Clear();
            sb.Append("2. tür\r\n----------------------------\r\n");
            string curstr = "item1+item2\\ deneme item3";
            //item1+item2\\ deneme döner
            // \\ özel karakteri kullanılarak girilen karakterler ayraca dahil edilmez, ek olarak özel karakteri metin içerisine de
            //yazdırmaya izin veridk
            curstr = curstr.Tokenize(StringQuoteOption.None, StringSplitOption.AllowSpecialChar | StringSplitOption.PrintSpecialCharacter , " ");
            sb.AppendFormat("sıraki: {0}\r\n", curstr);
            //item1 döner
            curstr = curstr.Tokenize("+");
            sb.AppendFormat("sıraki: {0}\r\n", curstr);
            MessageBox.Show(sb.ToString());
        }
        public class JsonTestClass
        {
            //name isimli etiketi bu değişkene atayacak
            [Json(TagName = "name")]
            public string Name { get; set; }

            [Json(TagName = "group")]
            public string Group { get; set; }

            public int Value { get; set; }

            //bu değişken üzerine bir atama gerçekleştirmeyecek.
            [Json(NotMapped = true)]
            public string Excluded { get; set; }
        }
        private void JsonTest()
        {
            //Proje içerisinde dâhili json ayrıştırıcı içerir, property isimleri için tırnak kullanımı şart koşulmaz
            //Ek olarak tek satırlı ve çok satırlı yorumları destekler, doğrudan bir sınıfa atanabilir/sınıftan jsona çevirilebilir.
            //ExplicitOpetör sayesinde doğrudan string ataması yapılabilir.
            //veya JsonItem item = JsonDecoder.Decode("{item1: 'değer', 'item2': 'değer'}");
            JsonItem item = "{item1: 'değer', 'item2': 'değer'}";
            Dictionary<string, object> keys = new Dictionary<string, object>();
            //Listeye sonradan bir veriyi JSON formatında ekledik.
            item.AddSubItem("{item3: 'değer', item4: 'değer'}");
            
            //Verilen nesnenin tipine göre içerisine aktarma yapar.
            item.ExportTo(ref keys);

            //Ayrıca dinamik olarakta aşağıdaki şekilde kullanılabilir.
            dynamic jitem = new JsonItem();
            jitem.Item1 = "değer1";
            jitem.Item2 = "değer2";
            jitem.Item3 = "değer3";
            jitem.Item4 = new JsonItem() {};
            jitem.Item4.Item1 = "değer2";
           
            string result = jitem.ToString();

            JsonItem sinifaAtanacak = "{name: 'macmillan', group: 'AR-GE', Value: 12345, Excluded: 'Bu değer atanmayacak'}";
            JsonTestClass testclass = new JsonTestClass();
            sinifaAtanacak.ExportTo(ref testclass);

            //Aynı sınıfı aşağıdaki şekilde JSON a çevirebilirsiniz.
            JsonItem jsitem = JsonDecoder.DecodeFrom(testclass);
            string jsonmetni = jsitem.ToString(); //Veya jsitem.ToJson();
        }

        private void StringExtensions()
        {
            //Yazılan metni url için uygun formata dönüştürür.
            string urlfriendly = "İcw macmillan İĞÜŞ".UrlFriendly();
            string str = "deneme";
            str.IsAllNumeric(); //Tüm karakterler rakammı(virgül/nokta içermesi durumunda false döner)
            str.IsBool(); //Metin bool türünden mi
            str.IsNumeric(); //Metin numerikmi
            str.IsIp(); //Ip adresi formatında mı
            str.IsPort(); //0 ila 65535 arasında ise true döner
            str.IsUrl(); //Metin url biçiminde mi
            str.IsMail(); //Metin e-posta adresi mi
            str.Escape(); //Boşluk vey alfanumerik karakterler haricindekileri _ ile değiştirir.
            str.GetFirst(3, "..."); //İlk 3 karaktarini dönderir(kalan kısmı ... olarak döner)
            //Metni şifreleme(AES kullanarak) 
            //Bu methodda kullanılacak varsayıla şifre
            CommonUtils.Extensions.StringExtensions.RetDataPass = "cwarge";
            //Şifreleme işlemi
            str.EncodeRetData();
            //Çözme işlemi
            str.DecodeRetData();

            //Varsayılan haricinde manuel şifre ile şifreleme/çözme
            str.EncryptAES("123456");
            str.DecryptAES("123456");


            //Rot13' e çevirme
            str.Rot13();

            //MD5' e çevirme
            str.ToMD5();

            //Başında ve sonundaki çift tırnakları kırpar
            str.RemoveQuota(); //

            //Çevirme işlemleri
            str.ToInt32();  //Int32
            str.ToInt64(); //Int64
            str.ToSingle();  //Single(Float)
            str.ToDouble(); //Double
            str.ToDecimal(); //Decimal
            str.ToDateTime(); //DateTime
            str.ToBool(); //Bool
            str.Fmt(1, 2, 3); //String.Format ile aynıdır

        }
        public class TestClass
        {
            public string Text { get; set; }
            public int Value { get; set; }
            public object SubObject { get; set; }
            
        }
      
        public static void ObjectAndReflectExtensions()
        {
            object obj = new TestClass();

            obj.IsNumericType(); //Nesnenin tipi numerik olan bir tipte ise true döner(float, int vs.)
            obj.IsObject(); //dictionary, array veya referans türü değilse false döner
            obj.IsEnum(); //değişken bir enum mu
            obj.IsArray(); //değişken bir dizi mi
            obj.IsDictionary(); //değişken bir dictionary mi

            obj.TransferTo(new TestClass()); //İçerisindeki değerleri kendisi ile aynı olan diğer sınıfa aktarmak için
            obj.TransferValuesTo(new TestClass(), "Text"); //Yukarıdaki ile aynı, sadece belirtilen alanların atamasını yapar.
            obj.GetMemberProtertyByPath("Text"); //Text isimli alanın Property infosunu getirdik
            obj.GetMemberProtertyByPath("SubObject.SubParam"); //Alt değişken sınıfının bir üyesinin Property Infosunu getirdik
            obj.GetMemberValueByPath("Text"); //obj.Text ile aynı sonucu döner.
        }
        public static void OtherExtensios()
        {
            //String Builder
            StringBuilder sb = new StringBuilder();
            sb.AppendLineFormat("{0}", "Deneme"); //AppendLine ile aynı, Formatlı bir biçimde son satıra ekler.

            //Dictionary
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict.AddOrUpdate("key", null); //Eğer varsa değiştirir, yoksa yenisini ekler.
            dict.GetValueDirect("key", null); //TryGetValue nin method şeklindeki hali


            List<string> list = new List<string>();
            list.AddRangeUntil(new string[] { "item1", "item2" }, (curlist, elem) => elem != "item1"); //Predicate true döndüğü anda listeye eklemeyi durdurur.
            list.AddRangeLoop(new string[] { "item1", "item2" }, (curlist, elem) => elem != "item1"); //Predicate true ise listeye ekler yoksa eklemez.


            //XElement ve XmlNode
            XmlNode test = null;
            test.GetAttribute("test"); //Elemanın bir attributesini getirir, yoksa default girilen değeri dönderir.
            test.GetAttributeInt("test"); //Yukarıdak ile aynı sonucu Int' e çevirir
            test.GetAttributeUntilNotNull("test"); //Elemana ait isimde girilen attribute bulunamazsa, üstündeki elemanın attributesini dönderir.
            test.GetValue("test"); //elamanı değerini yada varsayılanı dönderir
            test.GetValueInt("test"); //Yukarıdak ile aynı sonucu Int' e çevirir

            //DateTime
            DateTime dt = DateTime.Now;
            dt.ToStringify(); //1 dk önce vs gibi metne çevirir.

            //Lambda
            Expression<Func<string, bool>> lambda = (str) => true;
            lambda.LamdaOr(lambda); //2 lambda elemanını OR sınaması ile birleştirir, (Her iki sonuçtan birinin doğru olması sonuç true yapar)
            lambda.LamdaAnd(lambda); //Yukarıdaki ile aynı fakat true dönmesi için her 2 işlemin sonucuda true olması gereklidir.
        }
        public static void SomeOtherUtils()
        {
            //Generator
            //Öncelikle oluşturuyu init ettiriyoruz(Tek sefer yeterli)
            Generator.GeneratorInit();

            Generator.ConvertToIdChar(1); //Gİrilen sayısı A, B gibi çevirir.
            Generator.GenerateRndNumber(4); //Rasgele numara oluşturur
            Generator.GenerateRndString(4, Generator.GeneratorType.AllowLowerCaseChar | Generator.GeneratorType.AllowUpperCaseChar); //Rasgele metin oluşturur.

            //String
            StringHelper.Combine("path1", "path2"); //Path.Combine ile benzer \\ve / karakterini otomatik olarak kırpar.

            //Reflect
            ReflectUtil.AnonymToDictionary(new { test = 1 }); //Anonim neseyni dictionary ye çevirir
            ReflectUtil.MatchType("1245", typeof(int)); //Verilen değişkeni belirtilen tipe uyarlamaya çalışır.
        }




    }
}
