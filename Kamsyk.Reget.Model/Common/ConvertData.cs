using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Kamsyk.Reget.Model.Common {
    public sealed class ConvertData {
        #region Struct
        private struct StructRemovalMap {
            public string BaseLetter;
            public string Letters;

            public StructRemovalMap(string baseLetter, string letters) {
                BaseLetter = baseLetter;
                Letters = letters;
            }
        }
        #endregion

        #region Properties
        private static string _numberDecimalSeparator {
            get {
                return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            }
        }

        private static string _numberGroupSeparator {
            get {
                return System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            }
        }

        private static Hashtable _m_HtDiacriticsRemoval = null;
        private static Hashtable m_HtDiacriticsRemoval {
            get {
                if (_m_HtDiacriticsRemoval == null) {
                    _m_HtDiacriticsRemoval = GetDiacriticsHashtable();
                }

                return _m_HtDiacriticsRemoval;
            }
        }

        private static List<StructRemovalMap> _m_DefaultDiacriticsRemovalMap = null;
        private static List<StructRemovalMap> m_DefaultDiacriticsRemovalMap {
            get {
                if (_m_DefaultDiacriticsRemovalMap == null) {
                    _m_DefaultDiacriticsRemovalMap = GetRemovalMap();
                }

                return _m_DefaultDiacriticsRemovalMap;
            }
        }
        #endregion

        public static int ToInt32(object value) {
            try {
                if (value == null) return DataNulls.INT_NULL;

                return Convert.ToInt32(value);
            } catch {
                return DataNulls.INT_NULL;
            }
        }

        public static decimal ToDecimal(object value) {
            try {
                if (value == null) return DataNulls.DECIMAL_NULL;

                string strDec = value.ToString();
                if (strDec != null) {
                    strDec = strDec.Trim();
                    if (strDec.Length == 0) return DataNulls.DECIMAL_NULL;

                    strDec = strDec.Replace(_numberGroupSeparator, "").Replace(" ", "");

                    if (_numberDecimalSeparator == ".") {
                        strDec = strDec.Replace(",", ".");
                    }
                    if (_numberDecimalSeparator == ",") {
                        strDec = strDec.Replace(".", ",");
                    }
                    return Convert.ToDecimal(strDec);
                }
                return Convert.ToDecimal(strDec);
            } catch {
                return DataNulls.DECIMAL_NULL;
            }
        }

        public static double ToDouble(object value) {
            try {
                if (value == null) return DataNulls.DOUBLE_NULL;

                string strDec = value.ToString();
                if (strDec != null) {
                    strDec = strDec.Trim();
                    if (strDec.Length == 0) return DataNulls.DOUBLE_NULL;

                    if (_numberDecimalSeparator == ".") {
                        strDec = strDec.Replace(",", ".");
                    }
                    if (_numberDecimalSeparator == ",") {
                        strDec = strDec.Replace(".", ",");
                    }
                    return Convert.ToDouble(strDec);
                }
                return Convert.ToDouble(strDec);
            } catch {
                return DataNulls.DOUBLE_NULL;
            }
        }


        public static string ToDbDate(DateTime date, string dateType) {
            //System.Data.SqlTypes.SqlDateTime dSelectedDate = new System.Data.SqlTypes.SqlDateTime(date);
            //return dSelectedDate.ToSqlString().ToString();


            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            string strDate = "";
            if (dateType.Trim().ToUpper() == "CZ") {
                strDate = day + "/" + month + "/" + year;
            } else {
                strDate = month + "/" + day + "/" + year;
            }

            if (date.Hour > 0 || date.Minute > 0 || date.Second > 0) {
                strDate += " " + date.Hour.ToString() + ":" + date.Minute.ToString() + ":" + date.Second.ToString();
            }

            return strDate;


        }

        public static string ToDbDate(DateTime date) {
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            string strDate = year + "-" + month + "-" + day;

            if (date.Hour > 0 || date.Minute > 0 || date.Second > 0) {
                strDate += " " + date.Hour.ToString() + ":" + date.Minute.ToString() + ":" + date.Second.ToString();
            }

            return "CAST('" + strDate + "' AS datetime)";


        }

        public static string ToOracleDbDate(DateTime date) {
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            if (month.Length == 1) month = "0" + month;
            if (day.Length == 1) day = "0" + day;

            string strDate = year + "/" + month + "/" + day;

            string strFormat = "yyyy/mm/dd";
            if (date.Hour > 0 || date.Minute > 0 || date.Second > 0) {
                strDate += " " + date.Hour.ToString() + ":" + date.Minute.ToString() + ":" + date.Second.ToString();
                strFormat += " hh24:mi:ss";
            }

            return "to_DATE('" + strDate + "','" + strFormat + "')";


        }



        public static string ToTableSelectDate(DateTime date) {
            string year = date.Year.ToString();
            string month = date.Month.ToString();
            string day = date.Day.ToString();

            string strDate = "#" + month + "/" + day + "/" + year + "#";

            return strDate;


        }

        public static string ToDbDecimal(decimal number) {


            //return number.ToString().Replace(",",".");
            return number.ToString().Replace(_numberDecimalSeparator, ".");

        }

        public static DateTime ToDateFromDbUs(string strDate) {
            string[] strParseDate = strDate.Split('/');
            int year = ConvertData.ToInt32(strParseDate[2]);
            int month = ConvertData.ToInt32(strParseDate[0]);
            int day = ConvertData.ToInt32(strParseDate[1]);
            return new DateTime(year, month, day);
        }

        public static string ToSmallDateString(DateTime date) {
            return date.Day.ToString() + "." + date.Month.ToString() + "." + date.Year.ToString();
        }

        public static DateTime ToSmallDate(DateTime date) {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime ToDateTime(object date) {
            try {
                return Convert.ToDateTime(date);
            } catch {
                return DataNulls.DATETIME_NULL;
            }
        }

        public static DateTime ToDateFromCz(string strDate) {
            DateTime d = DataNulls.DATETIME_NULL;
            try {
                //filter out time
                strDate = strDate.Trim();
                if (strDate.IndexOf(" ") > -1) {
                    strDate = strDate.Substring(0, strDate.IndexOf(" "));
                }

                string[] strParseDate = strDate.Split('.');
                int year = ConvertData.ToInt32(strParseDate[2]);
                int month = ConvertData.ToInt32(strParseDate[1]);
                int day = ConvertData.ToInt32(strParseDate[0]);
                d = new DateTime(year, month, day);
            } catch {
                d = DataNulls.DATETIME_NULL;
            }

            return d;
        }

        public static DateTime ToDate(string strDate, string strFormat) {
            DateTime date;
            bool isDate =DateTime.TryParseExact(strDate, strFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);

            if (isDate) {
                return date;
            }

            return DataNulls.DATETIME_NULL;
        }

        public static string ToCzTextFromDate(DateTime date) {
            if (date == DataNulls.DATETIME_NULL) return "";
            //return date.Day.ToString() + "." + date.Month.ToString() + "." + date.Year.ToString();
            return String.Format("{0:dd.MM.yyyy}", date);
        }

        public static string ToTextFromDate(DateTime date) {
            if (date == DataNulls.DATETIME_NULL) return "";
            string strFormat = Thread.CurrentThread.CurrentCulture.DateTimeFormat.ShortDatePattern;
            
            return date.ToString(strFormat);
        }

        public static string ToCzTextFromTime(DateTime date) {
            if (date == DataNulls.DATETIME_NULL) return "";
            //return date.Day.ToString() + "." + date.Month.ToString() + "." + date.Year.ToString();
            return String.Format("{0:HH:mm:ss}", date);
        }

        public static string ToConcordeTextFromDate(DateTime date) {
            if (date == DataNulls.DATETIME_NULL) return "";
            //return date.Day.ToString() + "." + date.Month.ToString() + "." + date.Year.ToString();
            return String.Format("{0:yyyy-MM-dd}", date);
        }

        public static string ToCzTextFromDateTime(DateTime date) {
            if (date == DataNulls.DATETIME_NULL) return "";
            //return date.Day.ToString() + "." + date.Month.ToString() + "." + date.Year.ToString();
            return String.Format("{0:dd.MM.yyyy HH:mm:ss}", date);
        }

        public static string ToHtmlTextRows(string sourceText) {
            if (sourceText == null) return "";

            return sourceText.Replace("\r\n", "<br/>");
        }

        public static bool ToBoolean(object value) {
            try {
                if (value == null) {
                    return false;
                }

                return Convert.ToBoolean(value);
            } catch {
                return false;
            }
        }

        #region ToString
        public static string ToString(int value, int length) {
            string number = value.ToString();
            if (length > number.Length) {
                string addZero = "";
                for (int i = 0; i < length - number.Length; i++) {
                    addZero += "0";
                }
                number = addZero + number;
            }

            return number;
        }

        public static string ToString(object value) {

            return value.ToString();
        }

        public static string ToString(int value) {
            //NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            //nfi.NumberGroupSeparator = " ";

            //return value.ToString("N", nfi);
            return value.ToString();
        }

        public static string ToString(decimal value) {
            return ToString(value, DataNulls.INT_NULL, Thread.CurrentThread.CurrentCulture.Name, true);
        }

        public static string ToString(decimal value, string cultureName) {
            return ToString(value, DataNulls.INT_NULL, cultureName, true);
        }

        
        public static string ToString(decimal value, int decimalNumbers, string cultureName, bool isNumberGroupSeparator) {
            CultureInfo ci = new CultureInfo(cultureName);
            string decimalSeparator = ci.NumberFormat.NumberDecimalSeparator;
            string numberGroupSeparator = ci.NumberFormat.NumberGroupSeparator;
            //string decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            //string numberGroupSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;

            var crrNumSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberGroupSeparator;
            var crrDecSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            string strNumGrSep = (isNumberGroupSeparator) ? numberGroupSeparator : "";
            if (decimalNumbers >= 0) {
                return value.ToString("N" + decimalNumbers).Replace(crrNumSep, strNumGrSep).Replace(crrDecSep, decimalSeparator);
            } else {
                return value.ToString().Replace(crrNumSep, strNumGrSep).Replace(crrDecSep, decimalSeparator);
            }
        }

        public static string ToStringRemoveUselessZeros(decimal value, string cultureName, bool isNumberGroupSeparator) {
            string strNum = ToString(value, -1, cultureName, isNumberGroupSeparator);

            CultureInfo ci = new CultureInfo(cultureName);
            string decimalSeparator = ci.NumberFormat.NumberDecimalSeparator;
            if (strNum.Contains(decimalSeparator)) {
                bool isLastZero = true;
                while (isLastZero) {
                    string lastNumber = strNum.Substring(strNum.Length - 1);
                    if (lastNumber == "0") {
                        strNum = strNum.Substring(0, strNum.Length - 1);
                    } else {
                        isLastZero = false;
                    }
                }

                string lastFixNumber = strNum.Substring(strNum.Length - 1);
                if (lastFixNumber == decimalSeparator) {
                    strNum = strNum.Substring(0, strNum.Length - 1);
                }
            }

            return strNum;
        }

        public static string ToString(double value) {
            return ToString(value, DataNulls.INT_NULL);
        }

        public static string ToString(double value, int decimalNumbers) {
            string decimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (decimalNumbers > 0) {
                return value.ToString("N" + decimalNumbers).Replace(".", decimalSeparator);
            } else {
                return value.ToString().Replace(".", decimalSeparator);
            }
        }

        public static string ToString(byte[] source, Encoding encoding) {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms, encoding);
            bw.Write(source);

            try {
                ms.Flush();
                ms.Position = 0;
                StreamReader sr = new StreamReader(ms);
                string s = sr.ReadToEnd();

                return s;
            } catch {
                return null;
            } finally {
                bw.Close();
                ms.Close();
            }
        }

        public static string ToString(byte[] source) {
            return ToString(source, Encoding.UTF8);
        }

        public static string ToStringFromDateTimeLocal(DateTime dateTime) {
            return dateTime.ToString(Thread.CurrentThread.CurrentCulture);
        }

        public static string ToStringFromDateLocal(DateTime dateTime) {
            return dateTime.ToShortDateString();
        }

        public static string GetStringValue(decimal? dNumber, string strCurrCultureName, bool isNumberGroupSeparator) {
            return GetStringValue(dNumber, strCurrCultureName, isNumberGroupSeparator, -1);
        }

        public static string GetStringValue(decimal? dNumber, string strCurrCultureName, bool isNumberGroupSeparator, int decimalNumbers) {
            if (dNumber == null) {
                return "";
            }

            string strNumber = dNumber.ToString();
            string decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var regex = new System.Text.RegularExpressions.Regex("(?<=[\\" + decSep + "])[0-9]+");

            int iDecPartLength = 0;
            if (decimalNumbers < 0) {
                string decNumbers = "";
                if (regex.IsMatch(strNumber)) {
                    decNumbers = regex.Match(strNumber).Value;
                }
                iDecPartLength = decNumbers.Length;
            } else {
                iDecPartLength = decimalNumbers;
            }

            decimal tmpD = (Decimal)dNumber;
            string strRetNum = ConvertData.ToString(tmpD, iDecPartLength, strCurrCultureName, isNumberGroupSeparator);

            
            return strRetNum;
        }

        public static string GetStringValueRemoveUselessZeros(decimal? dNumber, string strCurrCultureName, bool isNumberGroupSeparator) {
            if (dNumber == null) {
                return "";
            }

            string strNumber = dNumber.ToString();
            string decSep = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            var regex = new System.Text.RegularExpressions.Regex("(?<=[\\" + decSep + "])[0-9]+");

            decimal tmpD = (Decimal)dNumber;
            string strRetNum = ConvertData.ToStringRemoveUselessZeros(tmpD, strCurrCultureName, isNumberGroupSeparator);


            return strRetNum;
        }
        #endregion

        public static string FormatNumber(string value, Type type) {
            value = value.Replace(",", ".");
            string retValue = value;

            if (value.Trim().Length == 0) return "";
            if (type != typeof(int) && type != typeof(decimal)) {
                return value;
            }

            try {
                if (type == typeof(int)) {
                    int i = Convert.ToInt32(value);

                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    nfi.NumberGroupSeparator = " ";
                    nfi.NumberDecimalDigits = 0;

                    retValue = i.ToString("N", nfi).Replace(".", ",");
                }
                if (type == typeof(decimal)) {
                    decimal d = Convert.ToDecimal(value);

                    NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                    nfi.NumberGroupSeparator = " ";

                    retValue = d.ToString("N", nfi).Replace(".", ",");
                }
            } catch {
                retValue = value;
            }

            return retValue;
        }

        public static string FormatNumber(int value) {
            string retValue = ConvertData.ToString(value);

            try {

                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                nfi.NumberGroupSeparator = " ";
                nfi.NumberDecimalDigits = 0;

                retValue = value.ToString("N", nfi).Replace(".", ",");


            } catch {
                retValue = ConvertData.ToString(value);
            }

            return retValue;
        }

        public static string FormatNumber(decimal value) {
            string retValue = ConvertData.ToString(value);

            try {
                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                nfi.NumberGroupSeparator = " ";
                //nfi.NumberDecimalDigits = 2;

                retValue = value.ToString("N", nfi).Replace(".", ",");


            } catch {
                retValue = ConvertData.ToString(value);
            }

            return retValue;
        }

        public static string FormatNumberToString(int number, int length) {
            string formatNumber = number.ToString();

            while (formatNumber.Length < length) {
                formatNumber = "0" + formatNumber;
            }

            return formatNumber;
        }

        public static byte[] ToByte(string text) {
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetBytes(text);

        }

        public static bool IsNumeric(string strNumber) {
            if (strNumber.Length == 0) return false;

            strNumber = strNumber.Replace(",", ".");
            try {
                decimal retNum;
                bool isNum = Decimal.TryParse(strNumber, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
                return (isNum);
                //return (strNumber == Convert.ToDecimal(strNumber).ToString());
            } catch {
                return false;
            }
        }

        public static bool IsInteger(string strNumber) {
            if (strNumber.Length == 0) return false;

            if (strNumber.IndexOf(",") > 0) {
                return false;
            }
            if (strNumber.IndexOf(".") > 0) {
                return false;
            }
            try {
                decimal retNum;
                bool isNum = Decimal.TryParse(strNumber, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
                return (isNum);
                //return (strNumber == Convert.ToDecimal(strNumber).ToString());
            } catch {
                return false;
            }
        }
                

        public static string GetFormatOtisBvNr(string phoneNumber) {
            if (phoneNumber == null) {
                return "";
            }

            string strPattern12 = @"(.)(\d{3})(\d{3})(\d{3})(\d{3})";
            string strReplacement12 = "$1$2 $3 $4 $5";

            string formatNumber = phoneNumber;
            formatNumber = formatNumber.Replace(" ", "");
            if (formatNumber.Length == 9 && !formatNumber.StartsWith("+420")) {
                formatNumber = "+420" + formatNumber;
            }
            if (formatNumber.Trim().Length == 13) {
                formatNumber = Regex.Replace(formatNumber, strPattern12, strReplacement12);
            }

            return formatNumber;
        }

        public static byte[] GetByteFromFile(string fullFileName) {
            byte[] byteContent = null;

            if (File.Exists(fullFileName)) {
                try {
                    FileInfo fi = new FileInfo(fullFileName);
                    FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    int fileLength = Convert.ToInt32(fi.Length);
                    byteContent = br.ReadBytes(fileLength);
                    br.Close();
                    fs.Close();
                } catch (Exception ex) {
                    throw (ex);
                }
            }

            return byteContent;
        }

        public static double ToKiloBytes(int bytes) {
            return bytes / 1024f;
        }



        public static bool IsMailAddressValid(string emailAddress) {
            string patternStrict = @"^(([^<>()[\]\\.,;:\s@\""]+"
                  + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                  + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                  + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                  + @"[a-zA-Z]{2,}))$";

            Regex reStrict = new Regex(patternStrict);
            string[] eMailAddresses = emailAddress.Split(';');
            foreach (string tmpMailAddress in eMailAddresses) {
                if (!reStrict.IsMatch(tmpMailAddress)) {
                    return false;
                }
            }
            return true;
        }

        #region Diacritics
        //public static string RemoveDiacritics(string rawText) {
        //    string stFormD = rawText.Normalize(NormalizationForm.FormD);
        //    StringBuilder sb = new StringBuilder();

        //    for (int ich = 0; ich < stFormD.Length; ich++) {
        //        UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
        //        if (uc != UnicodeCategory.NonSpacingMark) {
        //            sb.Append(stFormD[ich]);
        //        }
        //    }
        //    return (sb.ToString().Normalize(NormalizationForm.FormC));
        //}

        public static string RemoveDiacritics(string rawText) {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < rawText.Length; i++) {
                string letter = rawText.Substring(i, 1);
                if (m_HtDiacriticsRemoval.ContainsKey(letter)) {
                    sb.Append(m_HtDiacriticsRemoval[letter].ToString());
                } else {
                    sb.Append(letter);
                }
            }

            return sb.ToString();

            
        }

        private static Hashtable GetDiacriticsHashtable() {
            Hashtable ht = new Hashtable();

            for (int i = 0; i < m_DefaultDiacriticsRemovalMap.Count; i++) {
                string baseLetter = m_DefaultDiacriticsRemovalMap.ElementAt(i).BaseLetter;
                var letters = m_DefaultDiacriticsRemovalMap.ElementAt(i).Letters;
                foreach (var letter in letters) {
                    string strLetter = letter.ToString();
                    ht.Add(strLetter, baseLetter);
                }
            }

            return ht;
        }

        private static List<StructRemovalMap> GetRemovalMap() {
            List<StructRemovalMap> defaultDiacriticsRemovalMap = new List<StructRemovalMap>();

            //defaultDiacriticsRemovalMap.Add(new StructRemovalMap("A", "\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F"));
            //defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AA", "\uA732"));

            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("A", "\u0041\u24B6\uFF21\u00C0\u00C1\u00C2\u1EA6\u1EA4\u1EAA\u1EA8\u00C3\u0100\u0102\u1EB0\u1EAE\u1EB4\u1EB2\u0226\u01E0\u00C4\u01DE\u1EA2\u00C5\u01FA\u01CD\u0200\u0202\u1EA0\u1EAC\u1EB6\u1E00\u0104\u023A\u2C6F"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AA", "\uA732"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AE", "\u00C6\u01FC\u01E2"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AO", "\uA734"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AU", "\uA736"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AV", "\uA738\uA73A"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("AY", "\uA73C"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("B", "\u0042\u24B7\uFF22\u1E02\u1E04\u1E06\u0243\u0182\u0181"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("C", "\u0043\u24B8\uFF23\u0106\u0108\u010A\u010C\u00C7\u1E08\u0187\u023B\uA73E"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("D", "\u0044\u24B9\uFF24\u1E0A\u010E\u1E0C\u1E10\u1E12\u1E0E\u0110\u018B\u018A\u0189\uA779\u00D0"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("DZ", "\u01F1\u01C4"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Dz", "\u01F2\u01C5"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("E", "\u0045\u24BA\uFF25\u00C8\u00C9\u00CA\u1EC0\u1EBE\u1EC4\u1EC2\u1EBC\u0112\u1E14\u1E16\u0114\u0116\u00CB\u1EBA\u011A\u0204\u0206\u1EB8\u1EC6\u0228\u1E1C\u0118\u1E18\u1E1A\u0190\u018E"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("F", "\u0046\u24BB\uFF26\u1E1E\u0191\uA77B"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("G", "\u0047\u24BC\uFF27\u01F4\u011C\u1E20\u011E\u0120\u01E6\u0122\u01E4\u0193\uA7A0\uA77D\uA77E"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("H", "\u0048\u24BD\uFF28\u0124\u1E22\u1E26\u021E\u1E24\u1E28\u1E2A\u0126\u2C67\u2C75\uA78D"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("I", "\u0049\u24BE\uFF29\u00CC\u00CD\u00CE\u0128\u012A\u012C\u0130\u00CF\u1E2E\u1EC8\u01CF\u0208\u020A\u1ECA\u012E\u1E2C\u0197"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("J", "\u004A\u24BF\uFF2A\u0134\u0248"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("K", "\u004B\u24C0\uFF2B\u1E30\u01E8\u1E32\u0136\u1E34\u0198\u2C69\uA740\uA742\uA744\uA7A2"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("L", "\u004C\u24C1\uFF2C\u013F\u0139\u013D\u1E36\u1E38\u013B\u1E3C\u1E3A\u0141\u023D\u2C62\u2C60\uA748\uA746\uA780"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("LJ", "\u01C7"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Lj", "\u01C8"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("M", "\u004D\u24C2\uFF2D\u1E3E\u1E40\u1E42\u2C6E\u019C"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("N", "\u004E\u24C3\uFF2E\u01F8\u0143\u00D1\u1E44\u0147\u1E46\u0145\u1E4A\u1E48\u0220\u019D\uA790\uA7A4"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("NJ", "\u01CA"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Nj", "\u01CB"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("O", "\u004F\u24C4\uFF2F\u00D2\u00D3\u00D4\u1ED2\u1ED0\u1ED6\u1ED4\u00D5\u1E4C\u022C\u1E4E\u014C\u1E50\u1E52\u014E\u022E\u0230\u00D6\u022A\u1ECE\u0150\u01D1\u020C\u020E\u01A0\u1EDC\u1EDA\u1EE0\u1EDE\u1EE2\u1ECC\u1ED8\u01EA\u01EC\u00D8\u01FE\u0186\u019F\uA74A\uA74C"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OI", "\u01A2"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OO", "\uA74E"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OU", "\u0222"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("OE", "\u008C\u0152"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("oe", "\u009C\u0153"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("P", "\u0050\u24C5\uFF30\u1E54\u1E56\u01A4\u2C63\uA750\uA752\uA754"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Q", "\u0051\u24C6\uFF31\uA756\uA758\u024A"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("R", "\u0052\u24C7\uFF32\u0154\u1E58\u0158\u0210\u0212\u1E5A\u1E5C\u0156\u1E5E\u024C\u2C64\uA75A\uA7A6\uA782"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("S", "\u0053\u24C8\uFF33\u1E9E\u015A\u1E64\u015C\u1E60\u0160\u1E66\u1E62\u1E68\u0218\u015E\u2C7E\uA7A8\uA784"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("T", "\u0054\u24C9\uFF34\u1E6A\u0164\u1E6C\u021A\u0162\u1E70\u1E6E\u0166\u01AC\u01AE\u023E\uA786"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("TZ", "\uA728"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("U", "\u0055\u24CA\uFF35\u00D9\u00DA\u00DB\u0168\u1E78\u016A\u1E7A\u016C\u00DC\u01DB\u01D7\u01D5\u01D9\u1EE6\u016E\u0170\u01D3\u0214\u0216\u01AF\u1EEA\u1EE8\u1EEE\u1EEC\u1EF0\u1EE4\u1E72\u0172\u1E76\u1E74\u0244"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("V", "\u0056\u24CB\uFF36\u1E7C\u1E7E\u01B2\uA75E\u0245"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("VY", "\uA760"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("W", "\u0057\u24CC\uFF37\u1E80\u1E82\u0174\u1E86\u1E84\u1E88\u2C72"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("X", "\u0058\u24CD\uFF38\u1E8A\u1E8C"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Y", "\u0059\u24CE\uFF39\u1EF2\u00DD\u0176\u1EF8\u0232\u1E8E\u0178\u1EF6\u1EF4\u01B3\u024E\u1EFE"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("Z", "\u005A\u24CF\uFF3A\u0179\u1E90\u017B\u017D\u1E92\u1E94\u01B5\u0224\u2C7F\u2C6B\uA762"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("a", "\u0061\u24D0\uFF41\u1E9A\u00E0\u00E1\u00E2\u1EA7\u1EA5\u1EAB\u1EA9\u00E3\u0101\u0103\u1EB1\u1EAF\u1EB5\u1EB3\u0227\u01E1\u00E4\u01DF\u1EA3\u00E5\u01FB\u01CE\u0201\u0203\u1EA1\u1EAD\u1EB7\u1E01\u0105\u2C65\u0250"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("aa", "\uA733"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ae", "\u00E6\u01FD\u01E3"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ao", "\uA735"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("au", "\uA737"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("av", "\uA739\uA73B"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ay", "\uA73D"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("b", "\u0062\u24D1\uFF42\u1E03\u1E05\u1E07\u0180\u0183\u0253"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("c", "\u0063\u24D2\uFF43\u0107\u0109\u010B\u010D\u00E7\u1E09\u0188\u023C\uA73F\u2184"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("d", "\u0064\u24D3\uFF44\u1E0B\u010F\u1E0D\u1E11\u1E13\u1E0F\u0111\u018C\u0256\u0257\uA77A"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("dz", "\u01F3\u01C6"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("e", "\u0065\u24D4\uFF45\u00E8\u00E9\u00EA\u1EC1\u1EBF\u1EC5\u1EC3\u1EBD\u0113\u1E15\u1E17\u0115\u0117\u00EB\u1EBB\u011B\u0205\u0207\u1EB9\u1EC7\u0229\u1E1D\u0119\u1E19\u1E1B\u0247\u025B\u01DD"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("f", "\u0066\u24D5\uFF46\u1E1F\u0192\uA77C"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("g", "\u0067\u24D6\uFF47\u01F5\u011D\u1E21\u011F\u0121\u01E7\u0123\u01E5\u0260\uA7A1\u1D79\uA77F"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("h", "\u0068\u24D7\uFF48\u0125\u1E23\u1E27\u021F\u1E25\u1E29\u1E2B\u1E96\u0127\u2C68\u2C76\u0265"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("hv", "\u0195"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("i", "\u0069\u24D8\uFF49\u00EC\u00ED\u00EE\u0129\u012B\u012D\u00EF\u1E2F\u1EC9\u01D0\u0209\u020B\u1ECB\u012F\u1E2D\u0268\u0131"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("j", "\u006A\u24D9\uFF4A\u0135\u01F0\u0249"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("k", "\u006B\u24DA\uFF4B\u1E31\u01E9\u1E33\u0137\u1E35\u0199\u2C6A\uA741\uA743\uA745\uA7A3"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("l", "\u006C\u24DB\uFF4C\u0140\u013A\u013E\u1E37\u1E39\u013C\u1E3D\u1E3B\u017F\u0142\u019A\u026B\u2C61\uA749\uA781\uA747"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("lj", "\u01C9"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("m", "\u006D\u24DC\uFF4D\u1E3F\u1E41\u1E43\u0271\u026F"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("n", "\u006E\u24DD\uFF4E\u01F9\u0144\u00F1\u1E45\u0148\u1E47\u0146\u1E4B\u1E49\u019E\u0272\u0149\uA791\uA7A5"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("nj", "\u01CC"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("o", "\u006F\u24DE\uFF4F\u00F2\u00F3\u00F4\u1ED3\u1ED1\u1ED7\u1ED5\u00F5\u1E4D\u022D\u1E4F\u014D\u1E51\u1E53\u014F\u022F\u0231\u00F6\u022B\u1ECF\u0151\u01D2\u020D\u020F\u01A1\u1EDD\u1EDB\u1EE1\u1EDF\u1EE3\u1ECD\u1ED9\u01EB\u01ED\u00F8\u01FF\u0254\uA74B\uA74D\u0275"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("oi", "\u01A3"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("ou", "\u0223"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("oo", "\uA74F"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("p", "\u0070\u24DF\uFF50\u1E55\u1E57\u01A5\u1D7D\uA751\uA753\uA755"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("q", "\u0071\u24E0\uFF51\u024B\uA757\uA759"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("r", "\u0072\u24E1\uFF52\u0155\u1E59\u0159\u0211\u0213\u1E5B\u1E5D\u0157\u1E5F\u024D\u027D\uA75B\uA7A7\uA783"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("s", "\u0073\u24E2\uFF53\u00DF\u015B\u1E65\u015D\u1E61\u0161\u1E67\u1E63\u1E69\u0219\u015F\u023F\uA7A9\uA785\u1E9B"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("t", "\u0074\u24E3\uFF54\u1E6B\u1E97\u0165\u1E6D\u021B\u0163\u1E71\u1E6F\u0167\u01AD\u0288\u2C66\uA787"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("tz", "\uA729"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("u", "\u0075\u24E4\uFF55\u00F9\u00FA\u00FB\u0169\u1E79\u016B\u1E7B\u016D\u00FC\u01DC\u01D8\u01D6\u01DA\u1EE7\u016F\u0171\u01D4\u0215\u0217\u01B0\u1EEB\u1EE9\u1EEF\u1EED\u1EF1\u1EE5\u1E73\u0173\u1E77\u1E75\u0289"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("v", "\u0076\u24E5\uFF56\u1E7D\u1E7F\u028B\uA75F\u028C"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("vy", "\uA761"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("w", "\u0077\u24E6\uFF57\u1E81\u1E83\u0175\u1E87\u1E85\u1E98\u1E89\u2C73"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("x", "\u0078\u24E7\uFF58\u1E8B\u1E8D"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("y", "\u0079\u24E8\uFF59\u1EF3\u00FD\u0177\u1EF9\u0233\u1E8F\u00FF\u1EF7\u1E99\u1EF5\u01B4\u024F\u1EFF"));
            defaultDiacriticsRemovalMap.Add(new StructRemovalMap("z", "\u007A\u24E9\uFF5A\u017A\u1E91\u017C\u017E\u1E93\u1E95\u01B6\u0225\u0240\u2C6C\uA763"));


            return defaultDiacriticsRemovalMap;
        }
        #endregion

    }
}
