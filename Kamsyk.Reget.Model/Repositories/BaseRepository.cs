using Kamsyk.Reget.Model.Common;
using Kamsyk.Reget.Model.DataDictionary;
using Kamsyk.Reget.Model.ExtendedModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Repositories {
    public abstract class BaseRepository<T> where T : class {
        #region Constants
        public const string CULTURE_EN = "en-US";
        public const string CULTURE_CZ = "cs-CZ";
        public const string CULTURE_SK = "sk-SK";
        public const string CULTURE_PL = "pl-PL";
        public const string CULTURE_RO = "ro-RO";
        public const string CULTURE_BG = "bg-BG";
        public const string CULTURE_SL = "sl-SI";
        public const string CULTURE_SR = "sr-Latn-CS";
        public const string CULTURE_HU = "hu-HU";

        public const string FILTER_DATETIME_FORMAT = "M/d/yyyy";

        public const string DUPLICITY = "DUPLICITY";

        public const string URL_FROM_FILTER_DELIMITER = "from:";
        public const string URL_TO_FILTER_DELIMITER = "to:";
        #endregion

        //#region Struct
        //private struct StructRemovalMap {
        //    public string BaseLetter;
        //    public string Letters;

        //    public StructRemovalMap(string baseLetter, string letters) {
        //        BaseLetter = baseLetter;
        //        Letters = letters;
        //    }
        //}
        //#endregion

        #region Static Properties
        //private static List<StructRemovalMap> _m_DefaultDiacriticsRemovalMap = null;
        //private static List<StructRemovalMap> m_DefaultDiacriticsRemovalMap {
        //    get {
        //        if (_m_DefaultDiacriticsRemovalMap == null) {
        //            _m_DefaultDiacriticsRemovalMap = GetRemovalMap();
        //        }

        //        return _m_DefaultDiacriticsRemovalMap;
        //    }
        //}

        //private static Hashtable _m_HtDiacriticsRemoval = null;
        //private static Hashtable m_HtDiacriticsRemoval {
        //    get {
        //        if (_m_HtDiacriticsRemoval == null) {
        //            _m_HtDiacriticsRemoval = GetDiacriticsHashtable();
        //        }

        //        return _m_HtDiacriticsRemoval;
        //    }
        //}

        #endregion

        #region Properties
        protected InternalRequestEntities m_dbContext = new InternalRequestEntities();

        protected DbSet<T> DbSet {
            get; set;
        }

        protected string DefaultLanguage {
           get { return "en-US"; }
        }

        public static string UrlParamDelimiter {
            get { return "|"; }
        }
        public static string UrlParamValueDelimiter {
            get { return "~"; }
        }
        #endregion

        #region Constructor
        public BaseRepository() {
            DbSet = m_dbContext.Set<T>();
        }

        //private string m_IdName = "id";
        //protected virtual string IdName {
        //    get { return m_IdName; }
        //    set { m_IdName = value; }
        //}
        #endregion

        #region Statics Methods
        //private static List<StructRemovalMap> GetRemovalMap() {
        //    List<StructRemovalMap> defaultDiacriticsRemovalMap = new List<StructRemovalMap>();

        //    //defaultDiacriticsRemovalMap.Add(new StructRemovalMap("A", "\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F"));
        //    //defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AA", "\uA732"));

        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("A", "\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AA", "\uA732"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AE", "\u00C6\u01FC\u01E2"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AO", "\uA734"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AU", "\uA736"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AV", "\uA738\uA73A"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AY", "\uA73C"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("B", "\u0042\u24B7\uFF22\u1E02\u1E04\u1E06\u0243\u0182\u0181"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("C", "\u0043\u24B8\uFF23\u0106\u0108\u010A\u010C\u00C7\u1E08\u0187\u023B\uA73E"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("D", "\u0044\u24B9\uFF24\u1E0A\u010E\u1E0C\u1E10\u1E12\u1E0E\u0110\u018B\u018A\u0189\uA779\u00D0"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("DZ", "\u01F1\u01C4"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Dz", "\u01F2\u01C5"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("E", "\u0045\u24BA\uFF25\u00C8\u00C9\u00CA\u1EC0\u1EBE\u1EC4\u1EC2\u1EBC\u0112\u1E14\u1E16\u0114\u0116\u00CB\u1EBA\u011A\u0204\u0206\u1EB8\u1EC6\u0228\u1E1C\u0118\u1E18\u1E1A\u0190\u018E"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("F", "\u0046\u24BB\uFF26\u1E1E\u0191\uA77B"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("G", "\u0047\u24BC\uFF27\u01F4\u011C\u1E20\u011E\u0120\u01E6\u0122\u01E4\u0193\uA7A0\uA77D\uA77E"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("H", "\u0048\u24BD\uFF28\u0124\u1E22\u1E26\u021E\u1E24\u1E28\u1E2A\u0126\u2C67\u2C75\uA78D"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("I", "\u0049\u24BE\uFF29\u00CC\u00CD\u00CE\u0128\u012A\u012C\u0130\u00CF\u1E2E\u1EC8\u01CF\u0208\u020A\u1ECA\u012E\u1E2C\u0197"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("J", "\u004A\u24BF\uFF2A\u0134\u0248"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("K", "\u004B\u24C0\uFF2B\u1E30\u01E8\u1E32\u0136\u1E34\u0198\u2C69\uA740\uA742\uA744\uA7A2"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("L", "\u004C\u24C1\uFF2C\u013F\u0139\u013D\u1E36\u1E38\u013B\u1E3C\u1E3A\u0141\u023D\u2C62\u2C60\uA748\uA746\uA780"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("LJ", "\u01C7"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Lj", "\u01C8"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("M", "\u004D\u24C2\uFF2D\u1E3E\u1E40\u1E42\u2C6E\u019C"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("N", "\u004E\u24C3\uFF2E\u01F8\u0143\u00D1\u1E44\u0147\u1E46\u0145\u1E4A\u1E48\u0220\u019D\uA790\uA7A4"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("NJ", "\u01CA"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Nj", "\u01CB"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("O", "\u004F\u24C4\uFF2F\u00D2\u00D3\u00D4\u1ED2\u1ED0\u1ED6\u1ED4\u00D5\u1E4C\u022C\u1E4E\u014C\u1E50\u1E52\u014E\u022E\u0230\u00D6\u022A\u1ECE\u0150\u01D1\u020C\u020E\u01A0\u1EDC\u1EDA\u1EE0\u1EDE\u1EE2\u1ECC\u1ED8\u01EA\u01EC\u00D8\u01FE\u0186\u019F\uA74A\uA74C"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OI", "\u01A2"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OO", "\uA74E"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OU", "\u0222"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OE", "\u008C\u0152"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("oe", "\u009C\u0153"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("P", "\u0050\u24C5\uFF30\u1E54\u1E56\u01A4\u2C63\uA750\uA752\uA754"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Q", "\u0051\u24C6\uFF31\uA756\uA758\u024A"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("R", "\u0052\u24C7\uFF32\u0154\u1E58\u0158\u0210\u0212\u1E5A\u1E5C\u0156\u1E5E\u024C\u2C64\uA75A\uA7A6\uA782"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("S", "\u0053\u24C8\uFF33\u1E9E\u015A\u1E64\u015C\u1E60\u0160\u1E66\u1E62\u1E68\u0218\u015E\u2C7E\uA7A8\uA784"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("T", "\u0054\u24C9\uFF34\u1E6A\u0164\u1E6C\u021A\u0162\u1E70\u1E6E\u0166\u01AC\u01AE\u023E\uA786"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("TZ", "\uA728"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("U", "\u0055\u24CA\uFF35\u00D9\u00DA\u00DB\u0168\u1E78\u016A\u1E7A\u016C\u00DC\u01DB\u01D7\u01D5\u01D9\u1EE6\u016E\u0170\u01D3\u0214\u0216\u01AF\u1EEA\u1EE8\u1EEE\u1EEC\u1EF0\u1EE4\u1E72\u0172\u1E76\u1E74\u0244"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("V", "\u0056\u24CB\uFF36\u1E7C\u1E7E\u01B2\uA75E\u0245"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("VY", "\uA760"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("W", "\u0057\u24CC\uFF37\u1E80\u1E82\u0174\u1E86\u1E84\u1E88\u2C72"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("X", "\u0058\u24CD\uFF38\u1E8A\u1E8C"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Y", "\u0059\u24CE\uFF39\u1EF2\u00DD\u0176\u1EF8\u0232\u1E8E\u0178\u1EF6\u1EF4\u01B3\u024E\u1EFE"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Z", "\u005A\u24CF\uFF3A\u0179\u1E90\u017B\u017D\u1E92\u1E94\u01B5\u0224\u2C7F\u2C6B\uA762"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("a", "\u0061\u24D0\uFF41\u1E9A\u00E0\u00E1\u00E2\u1EA7\u1EA5\u1EAB\u1EA9\u00E3\u0101\u0103\u1EB1\u1EAF\u1EB5\u1EB3\u0227\u01E1\u00E4\u01DF\u1EA3\u00E5\u01FB\u01CE\u0201\u0203\u1EA1\u1EAD\u1EB7\u1E01\u0105\u2C65\u0250"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("aa", "\uA733"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ae", "\u00E6\u01FD\u01E3"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ao", "\uA735"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("au", "\uA737"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("av", "\uA739\uA73B"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ay", "\uA73D"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("b", "\u0062\u24D1\uFF42\u1E03\u1E05\u1E07\u0180\u0183\u0253"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("c", "\u0063\u24D2\uFF43\u0107\u0109\u010B\u010D\u00E7\u1E09\u0188\u023C\uA73F\u2184"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("d", "\u0064\u24D3\uFF44\u1E0B\u010F\u1E0D\u1E11\u1E13\u1E0F\u0111\u018C\u0256\u0257\uA77A"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("dz", "\u01F3\u01C6"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("e", "\u0065\u24D4\uFF45\u00E8\u00E9\u00EA\u1EC1\u1EBF\u1EC5\u1EC3\u1EBD\u0113\u1E15\u1E17\u0115\u0117\u00EB\u1EBB\u011B\u0205\u0207\u1EB9\u1EC7\u0229\u1E1D\u0119\u1E19\u1E1B\u0247\u025B\u01DD"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("f", "\u0066\u24D5\uFF46\u1E1F\u0192\uA77C"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("g", "\u0067\u24D6\uFF47\u01F5\u011D\u1E21\u011F\u0121\u01E7\u0123\u01E5\u0260\uA7A1\u1D79\uA77F"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("h", "\u0068\u24D7\uFF48\u0125\u1E23\u1E27\u021F\u1E25\u1E29\u1E2B\u1E96\u0127\u2C68\u2C76\u0265"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("hv", "\u0195"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("i", "\u0069\u24D8\uFF49\u00EC\u00ED\u00EE\u0129\u012B\u012D\u00EF\u1E2F\u1EC9\u01D0\u0209\u020B\u1ECB\u012F\u1E2D\u0268\u0131"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("j", "\u006A\u24D9\uFF4A\u0135\u01F0\u0249"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("k", "\u006B\u24DA\uFF4B\u1E31\u01E9\u1E33\u0137\u1E35\u0199\u2C6A\uA741\uA743\uA745\uA7A3"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("l", "\u006C\u24DB\uFF4C\u0140\u013A\u013E\u1E37\u1E39\u013C\u1E3D\u1E3B\u017F\u0142\u019A\u026B\u2C61\uA749\uA781\uA747"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("lj", "\u01C9"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("m", "\u006D\u24DC\uFF4D\u1E3F\u1E41\u1E43\u0271\u026F"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("n", "\u006E\u24DD\uFF4E\u01F9\u0144\u00F1\u1E45\u0148\u1E47\u0146\u1E4B\u1E49\u019E\u0272\u0149\uA791\uA7A5"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("nj", "\u01CC"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("o", "\u006F\u24DE\uFF4F\u00F2\u00F3\u00F4\u1ED3\u1ED1\u1ED7\u1ED5\u00F5\u1E4D\u022D\u1E4F\u014D\u1E51\u1E53\u014F\u022F\u0231\u00F6\u022B\u1ECF\u0151\u01D2\u020D\u020F\u01A1\u1EDD\u1EDB\u1EE1\u1EDF\u1EE3\u1ECD\u1ED9\u01EB\u01ED\u00F8\u01FF\u0254\uA74B\uA74D\u0275"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("oi", "\u01A3"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ou", "\u0223"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("oo", "\uA74F"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("p", "\u0070\u24DF\uFF50\u1E55\u1E57\u01A5\u1D7D\uA751\uA753\uA755"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("q", "\u0071\u24E0\uFF51\u024B\uA757\uA759"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("r", "\u0072\u24E1\uFF52\u0155\u1E59\u0159\u0211\u0213\u1E5B\u1E5D\u0157\u1E5F\u024D\u027D\uA75B\uA7A7\uA783"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("s", "\u0073\u24E2\uFF53\u00DF\u015B\u1E65\u015D\u1E61\u0161\u1E67\u1E63\u1E69\u0219\u015F\u023F\uA7A9\uA785\u1E9B"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("t", "\u0074\u24E3\uFF54\u1E6B\u1E97\u0165\u1E6D\u021B\u0163\u1E71\u1E6F\u0167\u01AD\u0288\u2C66\uA787"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("tz", "\uA729"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("u", "\u0075\u24E4\uFF55\u00F9\u00FA\u00FB\u0169\u1E79\u016B\u1E7B\u016D\u00FC\u01DC\u01D8\u01D6\u01DA\u1EE7\u016F\u0171\u01D4\u0215\u0217\u01B0\u1EEB\u1EE9\u1EEF\u1EED\u1EF1\u1EE5\u1E73\u0173\u1E77\u1E75\u0289"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("v", "\u0076\u24E5\uFF56\u1E7D\u1E7F\u028B\uA75F\u028C"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("vy", "\uA761"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("w", "\u0077\u24E6\uFF57\u1E81\u1E83\u0175\u1E87\u1E85\u1E98\u1E89\u2C73"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("x", "\u0078\u24E7\uFF58\u1E8B\u1E8D"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("y", "\u0079\u24E8\uFF59\u1EF3\u00FD\u0177\u1EF9\u0233\u1E8F\u00FF\u1EF7\u1E99\u1EF5\u01B4\u024F\u1EFF"));
        //    defaultDiacriticsRemovalMap.Add(new StructRemovalMap("z", "\u007A\u24E9\uFF5A\u017A\u1E91\u017C\u017E\u1E93\u1E95\u01B6\u0225\u0240\u2C6C\uA763"));


        //    return defaultDiacriticsRemovalMap;
        //}

        //private static Hashtable GetDiacriticsHashtable() {
        //    Hashtable ht = new Hashtable();

        //    for (int i = 0; i < m_DefaultDiacriticsRemovalMap.Count; i++) {
        //        string baseLetter = m_DefaultDiacriticsRemovalMap.ElementAt(i).BaseLetter;
        //        var letters = m_DefaultDiacriticsRemovalMap.ElementAt(i).Letters;
        //        foreach (var letter in letters) {
        //            string strLetter = letter.ToString();
        //            ht.Add(strLetter, baseLetter);
        //        }
        //    }

        //    return ht;
        //}

        public static string RemoveDiacritics(string rawText) {
            return ConvertData.RemoveDiacritics(rawText);
        //    StringBuilder sb = new StringBuilder();

        //    for (int i = 0; i < rawText.Length; i++) {
        //        string letter = rawText.Substring(i, 1);
        //        if (m_HtDiacriticsRemoval.ContainsKey(letter)) {
        //            sb.Append(m_HtDiacriticsRemoval[letter].ToString());
        //        } else {
        //            sb.Append(letter);
        //        }
        //    }

        //    return sb.ToString();

        //    //string stFormD = rawText.Normalize(NormalizationForm.FormD);
        //    //StringBuilder sb = new StringBuilder();

        //    //for (int ich = 0; ich < stFormD.Length; ich++) {
        //    //    string letter = stFormD.Substring(ich, 1);
        //    //    switch (letter) {
        //    //        case "ą":
        //    //            sb.Append("a");
        //    //            break;
        //    //        case "ć":
        //    //            sb.Append("c");
        //    //            break;
        //    //        case "ę":
        //    //            sb.Append("e");
        //    //            break;
        //    //        case "ł":
        //    //            sb.Append("l");
        //    //            break;
        //    //        case "Ł":
        //    //            sb.Append("L");
        //    //            break;
        //    //        case "ń":
        //    //            sb.Append("n");
        //    //            break;
        //    //        case "ó":
        //    //            sb.Append("o");
        //    //            break;
        //    //        case "ś":
        //    //            sb.Append("s");
        //    //            break;
        //    //        case "ż":
        //    //        case "ź":
        //    //            sb.Append("z");
        //    //            break;
        //    //        default:
        //    //            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
        //    //            if (uc != UnicodeCategory.NonSpacingMark) {
        //    //                sb.Append(stFormD[ich]);
        //    //            }
        //    //            break;
        //    //    }


        //    //}
        //    //return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        public static string RemoveDiacriticsWhiteSpace(string rawText) {
            string retText = RemoveDiacritics(rawText).Replace(" ", "");

            return retText;
        }
       
        #endregion

        #region Methods
        protected void SaveChanges() {
            m_dbContext.SaveChanges();
        }

        //public int GetLastId(string idColName) {
        //    string sqlQuery = "SELECT TOP (1) " + idColName + " FROM " + "useraccount" +
        //        " ORDER BY " + idColName + " DESC";
        //    int iLastId = m_dbContext.Database.SqlQuery<int>(sqlQuery).Single();

        //    return iLastId;
        //}

        //public int GetLastId() {
        //    m_dbContext.Purchase_Group_Limit.Max(x => x.limit_id);
        //    //return GetLastId(m_IdName);
        //}

        public T GetFirstOrDefault(Expression<Func<T, bool>> predicate) {
            return DbSet.FirstOrDefault(predicate);
                       
        }

        //public List<T> ToList() {
        //    return DbSet.ToList();

        //}

        public void Add(T entity) {
            DbSet.Add(entity);
        }

        //public T SqlQuery(string sqlQuery) {
        //    var data = DbSet.SqlQuery(sqlQuery).SingleOrDefault<T>();

        //    return data;
        //}

        protected void ErrorHandle(Exception ex) {
            var entityValidationErrors = ((System.Data.Entity.Validation.DbEntityValidationException)ex).EntityValidationErrors;

            throw ex;
        }

        public static void SetValues(object sourceObject, object targetObject) {
            SetValues(sourceObject, targetObject, null, false);
        }

        protected static void SetValues(object sourceObject, object targetObject, List<string> properties) {
            SetValues(sourceObject, targetObject, properties, false);
        }

        protected static void SetValues(object sourceObject, object targetObject, List<string> properties, bool isRecursive) {
            Type tSource = sourceObject.GetType();
            Type tTarget = targetObject.GetType();

            PropertyInfo[] sourceAttributes = tSource.GetProperties();
            PropertyInfo[] targetAttributes = tTarget.GetProperties();

            foreach (PropertyInfo sourceAttribute in sourceAttributes) {
                if (properties != null && !properties.Contains(sourceAttribute.Name)) {
                    continue;
                }
                                
                PropertyInfo targetProperty = tTarget.GetProperty(sourceAttribute.Name);
                if (targetProperty == null) {
                    continue;
                }


                if (sourceAttribute.PropertyType.FullName.IndexOf("Kamsyk.Reget.Model") > -1) {
                    if (isRecursive) {
                        SetValues(sourceAttribute.GetValue(sourceObject, null), targetProperty.GetValue(targetObject, null), null, true);
                    }
                    continue;
                }

                object oSourceValue = sourceAttribute.GetValue(sourceObject, null);
                targetProperty.SetValue(targetObject, oSourceValue, null);
            }
            

        }

        protected string GetSqlIn(List<int> items) {
            string sqlIn = "";

            foreach (int i in items) {
                if (sqlIn.Length > 0) {
                    sqlIn += ",";
                }

                sqlIn += i;
            }

            return "(" + sqlIn + ")";
        }

        protected void AddMaintenanceEvent(
           EventRepository.EventCode eventCode,
           string eventMsg,
           int companyId,
           string companyName,
           int cgId,
           string cgName,
           int centreId,
           string centreName,
           int pgId,
           string pgName,
           int modifUserId,
           ref string errMsg) {
            
            Maintenance_Event me = new Maintenance_Event();
            try {
                var comp = (from compDb in m_dbContext.Company
                            where compDb.id == companyId
                            select compDb).FirstOrDefault();
                if (comp == null || comp.is_event_log_enabled == null || comp.is_event_log_enabled == false) {
                    return;
                }

                //check data
                eventMsg = ShortString(eventMsg, MaintenanceEventData.MSG_TEXT_LENGTH);
                companyName = ShortString(companyName, MaintenanceEventData.COMPANY_NAME_LENGTH);
                cgName = ShortString(cgName, MaintenanceEventData.CENTRE_GROUP_NAME_LENGTH);
                centreName = ShortString(centreName, MaintenanceEventData.CENTRE_GROUP_NAME_LENGTH);
                pgName = ShortString(pgName, MaintenanceEventData.PURCHASE_GROUP_NAME_LENGTH);

                if (eventMsg.Length > MaintenanceEventData.MSG_TEXT_LENGTH) {
                    eventMsg = null;
                }

                                
                me.msg_code = (int)eventCode;
                me.msg_text = eventMsg;
                me.company_id = companyId;
                me.company_name = companyName;
                me.centre_group_id = cgId;
                me.centre_group_name = cgName;
                me.centre_id = centreId;
                me.centre_name = centreName;
                me.purchase_group_id = pgId;
                me.purchase_group_name = pgName;
                me.modif_user = modifUserId;
                me.modif_date = DateTime.Now;

                int lastId = -1;
                var lastEvent = (from evDb in m_dbContext.Maintenance_Event
                                 orderby evDb.id descending
                                 select new { id = evDb.id }).FirstOrDefault();
                if (lastEvent != null) {
                    lastId = lastEvent.id;
                }

                int newId = lastId + 1;
                me.id = newId;

                
                m_dbContext.Maintenance_Event.Add(me);
                m_dbContext.SaveChanges();
            } catch(Exception ex) {
                try {
                    m_dbContext.Entry(me).State = EntityState.Detached;
                } catch {
                    if (errMsg == null) {
                        errMsg = "";
                    }
                    if (errMsg.Length > 0) {
                        errMsg += Environment.NewLine;
                    }

                    errMsg += "Detach event Row failed. " + ex.Message + "Code:" + (int)eventCode + " User Id:" + modifUserId + " Message:" + eventMsg;
                }

                if (errMsg == null) {
                    errMsg = "";
                }
                if (errMsg.Length > 0) {
                    errMsg += Environment.NewLine;
                }

                errMsg += "Code:" + (int)eventCode + " User Id:" + modifUserId + " Message:" + eventMsg + ". " + ex.Message;
               
            }
        }

        protected string ShortString(string strText, int maxLenght) {
            if (strText == null) {
                return null;
            }

            if (strText.Length <= maxLenght) {
                return strText;
            }

            string shortText = strText.Substring(0, maxLenght - 3) + "...";
            return shortText;
        }

        protected string GetCountryFlag(int companyId, string urlRoot) {
            switch (companyId) {
                case 0:
                case 2:
                    return urlRoot + "Content/Images/Culture/cz.gif";
                case 1:
                    return urlRoot + "Content/Images/Culture/sk.gif";
                case 3:
                    return urlRoot + "Content/Images/Culture/ro.gif";
                case 4:
                    return urlRoot + "Content/Images/Culture/pl.gif";
                case 5:
                    return urlRoot + "Content/Images/Culture/bg.gif";
                case 6:
                    return urlRoot + "Content/Images/Culture/sl.gif";
                case 7:
                    return urlRoot + "Content/Images/Culture/sr.gif";
                case 8:
                    return urlRoot + "Content/Images/Culture/hu.gif";
                case 9:
                    return urlRoot + "Content/Images/Culture/uk.gif";
                default:
                    return urlRoot + "Content/Images/Culture/empty.gif"; ;
            }
        }

        protected string GetPgLabel(string cultureName) {
            string cultureNameLow = cultureName.ToLower();
            switch (cultureNameLow) {
                case "cs-cz":
                    return "Česky";
                case "sk-sk":
                    return "Slovensky";
                case "ro-ro":
                    return "Roman";
                case "pl-pl":
                    return "Polski";
                case "bg-bg":
                    return "български";
                case "sl-sl":
                    return "Slovenski";
                case "sr-latn-cs":
                    return "Srpski";
                case "hu-hu":
                    return "Magyar";
                default:
                    return "English";
            }

        }

        protected string GetPgFlagUrl(string cultureName, string rootUrl) {
            string cultureNameLow = cultureName.ToLower();
            switch (cultureNameLow) {
                case "cs-cz":
                    return rootUrl + "Content/Images/Culture/cz.gif";
                case "sk-sk":
                    return rootUrl + "Content/Images/Culture/sk.gif";
                case "ro-ro":
                    return rootUrl + "Content/Images/Culture/ro.gif";
                case "pl-pl":
                    return rootUrl + "Content/Images/Culture/pl.gif";
                case "bg-bg":
                    return rootUrl + "Content/Images/Culture/bg.gif";
                case "sl-sl":
                    return rootUrl + "Content/Images/Culture/sl.gif";
                case "sr-latn-cs":
                    return rootUrl + "Content/Images/Culture/sr.gif";
                case "hu-hu":
                    return rootUrl + "Content/Images/Culture/hu.gif";
                default:
                    return rootUrl + "Content/Images/Culture/uk.gif";
            }

        }

        protected void HandleDbError(Exception ex) {
        }
        #endregion
    }
}

