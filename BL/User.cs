using DL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BL
{
    public class User
    {

        public ML.Result GetById(string id)
        {
            ML.Result result = new ML.Result();

            var query = DL.EJanBD.users.SingleOrDefault(user => user.id == id);

            if (query != null)
            {
                result.Object = query;
                result.Correct = true;//quiere decir que si existe ese usuario con ese ID
            }
            else
            {
                result.Correct = false;//quiere decir que no existe y se puede agregar
            }

            return result;
        }

        public ML.Result Add(ML.User user)
        {
            ML.Result result = new ML.Result();

            //agregar el id de guid
            user.id = Guid.NewGuid().ToString();

            //validar que el usaurio no se repita por el id
            ML.Result resultGetById = GetById(user.id);

            if (!resultGetById.Correct)
            {
                //Agreagr la fecha
                var zonaMadagascar = TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time");
                var fechaMadagascar = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    zonaMadagascar
                );
                user.created_at = fechaMadagascar.ToString("dd-MM-yyyy HH:mm");

                //Agregar la contraseña encriptada
                user.password = EncryptionHelper.Encrypt(user.password);

                DL.EJanBD.users.Add(user);

                user.password = null;

                result.Object = user;
                result.Correct = true;
                result.status = 201;

            }
            else
            {
                result.Correct = false;
                result.status = 500;
                result.ErrorMessage = "Este usuario ya existe";
            }


            return result;

        }


    }
}
