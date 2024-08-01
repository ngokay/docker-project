namespace WebApplication5.DependencyInjection
{
    public interface IClassA
    {
    }
    public class ClassA : IClassA
    {
        /// <summary>
        /// Contructor default
        /// </summary>
        public ClassA()
        {
            Console.WriteLine("Class A:"+ Guid.NewGuid().ToString());
        }
    }
}
