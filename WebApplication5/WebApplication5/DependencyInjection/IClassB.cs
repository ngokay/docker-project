namespace WebApplication5.DependencyInjection
{
    public interface IClassB
    {
    }
    public class ClassB : IClassB
    {
        private readonly IClassA _classA;
        /// <summary>
        /// Contructor default
        /// </summary>
       
        public ClassB()
        {
            Console.WriteLine("Class B :" + Guid.NewGuid().ToString());
        }     
    }
}
