using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YiFramework.Core
{
   public class Util
    {


        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }

        /// <summary>
        /// 将指定串加密为不包含中杠的MD5值
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="isupper">返回值的大小写(true大写,false小写)</param>
        /// <returns></returns>
        public static string ComputeMD5(string str, bool isupper)
        {
            string md5str = ComputeMD5(str);
            if (isupper)
                return md5str.ToUpper();
            return md5str.ToLower();
        }

        /// <summary>
        /// 利用 MD5 加密算法加密字符串
        /// </summary>
        /// <param name="src">字符串源串</param>
        /// <returns>返加MD5 加密后的字符串</returns>
        public static string ComputeMD5(string src)
        {
            //将密码字符串转化成字节数组
            byte[] byteArray = GetByteArray(src);

            //计算 MD5 密码
            byteArray = (new MD5CryptoServiceProvider().ComputeHash(byteArray));

            //将字节码转化成字符串并返回
            return BitConverter.ToString(byteArray).Replace("-", "");
        }

        /// <summary>
        /// 将字符串翻译成字节数组
        /// </summary>
        /// <param name="src">字符串源串</param>
        /// <returns>字节数组</returns>
        private static byte[] GetByteArray(string src)
        {
            byte[] byteArray = new byte[src.Length];

            for (int i = 0; i < src.Length; i++)
            {
                byteArray[i] = Convert.ToByte(src[i]);
            }

            return byteArray;
        }

       /// <summary>
       /// 比较两个byte数组
       /// </summary>
       /// <param name="arr1"></param>
       /// <param name="arr2"></param>
       /// <returns></returns>
       public static bool EqualByteArray(byte[] arr1, byte[] arr2)
       {
           if (arr1 == null || arr2 == null) { return true; }
           if (arr1.Length != arr2.Length) { return false; }
           bool flag = true;
           for (int i = arr1.Length - 1; i > -1; i--)
           {
               if (arr1[i] != arr2[i]) { flag = false; break; }
           }
           return flag;
       }
    }
}
