using System.ComponentModel.DataAnnotations;

namespace ByteStoreAPI.Entity
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Category(string name) 
        {
            this.Id = Guid.NewGuid();
            this.Name = name;
        }
        private Category()
        { }
    }
}
