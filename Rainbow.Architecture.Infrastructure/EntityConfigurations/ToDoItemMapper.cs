using Rainbow.Architecture.Domain.AggregatesModel.ToDoAggregate.Entities;
using Rainbow.DapperExtensions.Mapper;

namespace Rainbow.Architecture.Infrastructure.EntityConfigurations
{
    public class ToDoItemMapper : ClassMapper<ToDoItem>
    {
        public ToDoItemMapper()
        {
            Table("t_todo");
            Map(p => p.Id).Key(KeyType.Guid);
            Map(p => p.IsDone).Column("is_done");
            AutoMap();
        }
    }
}
