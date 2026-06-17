using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace BL
{
    public class Login
    {
        public ML.Result Logearse(ML.Login login)
        {
            ML.Result result = new ML.Result();

            //obtener 
            ML.User query = DL.EJanBD.users.SingleOrDefault(user => user.tax_id == login.tax_id);

            if (query != null)
            {
                //Encriptar
                string passwordEncriptadaU = EncryptionHelper.Encrypt(login.password);

                if (query.password == passwordEncriptadaU)
                {
                    result.Correct = true;
                }
                else
                {
                    result.Correct = false;
                    result.status = 400;
                    result.ErrorMessage = "la contrsenia es incorrecta";
                }

            }
            else
            {
                result.Correct = false;
                result.status = 400;
                result.ErrorMessage = "Este usuario no existe";
            }

            return result;
        }
    }
}
