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
        public ML.Result GetAll(string? filter, string? sortedBy)
        {
            ML.Result result = new ML.Result();

            //traer toda la informacion
            var query = DL.EJanBD.users.ToList();

            IEnumerable<ML.User> resultado = query;

            /*validar ahora sorted by, si no viene nulo que haga la ordenacion, si viene nulo dejamos la lista*/
            if (!string.IsNullOrWhiteSpace(sortedBy))
            {
                switch (sortedBy)
                {
                    case "email":
                        resultado = query.OrderBy(u => u.email);
                        break;
                    case "id":
                        resultado = query.OrderBy(u => u.id);
                        break;
                    case "name":
                        resultado = query.OrderBy(u => u.name);
                        break;
                    case "phone":
                        resultado = query.OrderBy(u => u.phone);
                        break;
                    case "created at":
                        resultado = query.OrderBy(u => u.created_at);
                        break;

                }
            }


            //separar el filtro en los 3 parametros
            string[] parametros = filter.Split('+', 3);

            //filtrar la lista dependiendo de si sorted by venga vacio o no, VACIO = Lista por default si no con la lista ordenada
            var resultadoFiltrado = (parametros[0], parametros[1]) switch
            {
                ("email", "co") => resultado.Where(x => x.email.Contains(parametros[2])),
                ("email", "eq") => resultado.Where(x => x.email == parametros[2]),
                ("email", "sw") => resultado.Where(x => x.email.StartsWith(parametros[2])),
                ("email", "ew") => resultado.Where(x => x.email.EndsWith(parametros[2])),

                ("name", "co") => resultado.Where(x => x.name.Contains(parametros[2])),
                ("name", "eq") => resultado.Where(x => x.name == parametros[2]),
                ("name", "sw") => resultado.Where(x => x.name.StartsWith(parametros[2])),
                ("name", "ew") => resultado.Where(x => x.name.EndsWith(parametros[2])),

                ("phone", "co") => resultado.Where(x => x.phone.Contains(parametros[2])),
                ("phone", "eq") => resultado.Where(x => x.phone == parametros[2]),
                ("phone", "sw") => resultado.Where(x => x.phone.StartsWith(parametros[2])),
                ("phone", "ew") => resultado.Where(x => x.phone.EndsWith(parametros[2])),

                ("id", "co") => resultado.Where(x => x.id.Contains(parametros[2])),
                ("id", "eq") => resultado.Where(x => x.id == parametros[2]),
                ("id", "sw") => resultado.Where(x => x.id.StartsWith(parametros[2])),
                ("id", "ew") => resultado.Where(x => x.id.EndsWith(parametros[2])),

                ("created at", "co") => resultado.Where(x => x.created_at.Contains(parametros[2])),
                ("created at", "eq") => resultado.Where(x => x.created_at == parametros[2]),
                ("created at", "sw") => resultado.Where(x => x.created_at.StartsWith(parametros[2])),
                ("created at", "ew") => resultado.Where(x => x.created_at.EndsWith(parametros[2]))
            };


            if (resultadoFiltrado.ToList().Count > 0)
            {
                result.Objects = resultadoFiltrado.ToList();
                result.Correct = true;
                result.status = 200;
            }
            else
            {
                result.ErrorMessage = "No hay registros con este filtrado";
                result.Correct = false;
                result.status = 404;
            }

            return result;
        }

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

            //validar que no re repita tax_id, osea unique
            if (DL.EJanBD.users.Any(u => u.tax_id == user.tax_id))
            {
                result.Correct = false;
                result.status = 400;
                result.ErrorMessage = "El Tax_id proporcionado ya existe";
                return result;
            }

            //agregar el id de guid
            user.id = Guid.NewGuid().ToString();

            //validar que el usaurio no se repita por el id
            ML.Result resultGetById = GetById(user.id);

            if (!resultGetById.Correct)
            {
                
                if (!Regex.IsMatch(user.tax_id, @"^([A-ZÑ&]{3,4})\d{6}([A-Z0-9]{3})$")) {
                    result.ErrorMessage += "Formato invalido de tax_id,";
                }

                if (!Regex.IsMatch(user.phone, @"^(\+?\d{1,3})?\d{10}$")) {
                    result.ErrorMessage += "Formato invalido de telefono";
                }

                if (result.ErrorMessage != null) {
                    result.Correct = false;
                    result.status = 400;
                    return result;
                }

                //Agreagr la fecha
                var zonaMadagascar = TimeZoneInfo.FindSystemTimeZoneById("E. Africa Standard Time");
                var fechaMadagascar = TimeZoneInfo.ConvertTimeFromUtc(
                    DateTime.UtcNow,
                    zonaMadagascar
                );
                user.created_at = fechaMadagascar.ToString("dd-MM-yyyy HH:mm");

                //Agregar la contraseña encriptada
                user.password = EncryptionHelper.Encrypt(user.password);

                //Agrear el id de a la direccion
                if (user.addresses != null)
                {
                    int contador = 1; 
                    foreach (ML.Address address in user.addresses) {
                        address.id = contador++;
                    }
                    
                }

                DL.EJanBD.users.Add(user);

                ML.User userAgregado = new ML.User
                {
                    id = user.id,
                    email = user.email,
                    name = user.name,
                    phone = user.phone,
                    tax_id = user.tax_id,
                    created_at = user.created_at,
                    addresses = user.addresses
                };


                user.password = null;

                result.Object = userAgregado;
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

        public ML.Result PartialUpdate(ML.User user, string id)
        {
            ML.Result result = new ML.Result();

            ML.Result resultGetById = GetById(id);

            if (resultGetById != null)
            {
                ML.User findUser = (ML.User)resultGetById.Object;

                var propiedades = typeof(ML.User).GetProperties();//traer las propiedadades del modelo de usuario

                foreach (var propiedad in propiedades)//recorre cada propiedad del modelo de usuario
                {
                    var value = propiedad.GetValue(user);//a apartir  de la propiedad leemos lo que envio el el usuario desde la api

                    if (value != null)
                    {
                        propiedad.SetValue(findUser, value);//en esa propiedad coloca lo que envio el usuario desde la api
                    }
                }
            }
            else
            {
                result.Correct = false;
                result.ErrorMessage = "Este usuario no existe";
                result.status = 404;
            }


            return result;
        }


    }
}
