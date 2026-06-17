namespace PruebasUnitarias
{
    public class UnitTest1
    {
        [Fact]
        public void ValidarLosTresUsuarios()
        {
            // Assert
            Assert.Equal(3, DL.EJanBD.users.Count);
        }

        [Fact]
        public void Logearse_UsuarioYPasswordCorrectos_DebeRetornarCorrect()
        {

            var login = new ML.Login
            {
                tax_id = "ABC850624XYZ",
                password = "Juan456"
            };

            var blLogin = new BL.Login();

            // Act
            ML.Result result = blLogin.Logearse(login);

            // Assert
            Assert.True(result.Correct);
        }
    }
}