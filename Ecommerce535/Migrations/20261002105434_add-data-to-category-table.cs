using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce535.Migrations
{
    /// <inheritdoc />
    public partial class adddatatocategorytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (name, description, status) values ('Mobiles', 'Morbi vestibulum, velit id pretium iaculis, diam erat fermentum justo, nec condimentum neque sapien placerat ante.', 0);insert into Categories (name, description, status) values ('Laptops', 'Mauris enim leo, rhoncus sed, vestibulum sit amet, cursus id, turpis.', 0);insert into Categories (name, description, status) values ('Cameras', 'Donec ut dolor.', 1);insert into Categories (name, description, status) values ('TVs', 'Integer ac neque.', 0);insert into Categories (name, description, status) values ('Tablets', 'Fusce lacus purus, aliquet at, feugiat non, pretium quis, lectus.', 0);"); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Categories"); 
        }
    }
}
