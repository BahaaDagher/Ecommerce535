using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce535.Migrations
{
    /// <inheritdoc />
    public partial class adddatatobrandtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Brands (name, description, status) values ('Apple', 'Duis aliquam convallis nunc.', 1);insert into Brands (name, description, status) values ('Oppo', 'In est risus, auctor sed, tristique in, tempus sit amet, sem.', 0);insert into Brands (name, description, status) values ('Realme', 'Fusce congue, diam id ornare imperdiet,', 0);insert into Brands (name, description, status) values ('Nokia', 'In eleifend quam a odio.', 0);insert into Brands (name, description, status) values ('HP', 'Fusce consequat.', 1);"); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("delete from Brands");
        }
    }
}
