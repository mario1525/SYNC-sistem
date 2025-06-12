using Data;
using Entity;


namespace Services
{
    public class CuadLogical
    {
        private readonly Dao_Cuad _daoCuad;

        public CuadLogical(Dao_Cuad daoCuad)
        {
            _daoCuad = daoCuad;

        }

        public async Task<List<Cuad>> Getcuad(Cuad cuad)
        {
            return await _daoCuad.GetCuad(cuad);
        }

        public Mensaje CreateCuad(Cuad Cuad)
        {
            Guid uid = Guid.NewGuid();
            Cuad.Id = uid.ToString();
            _daoCuad.SetCuad("I", Cuad);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = uid.ToString();
            return mensaje;

        }

        public Mensaje UpdateCuad(Cuad Cuad)
        {
            _daoCuad.SetCuad("A", Cuad);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "Cuad actualizado";
            return mensaje;

        }

        public Mensaje DeleteCuad(string Id)
        {
            _daoCuad.Deletecuad(Id);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "Cuad eliminado";
            return mensaje;

        }

        public Mensaje ActiveCuad(string Id, int estado)
        {
            _daoCuad.Activecuad(Id, estado);
            Mensaje mensaje = new Mensaje();
            mensaje.mensaje = "se cambio el estado del Cuad";
            return mensaje;

        }
    }
}