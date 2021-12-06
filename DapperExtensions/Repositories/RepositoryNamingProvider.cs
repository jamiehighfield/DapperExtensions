using Dapper;
using DapperExtensions.Entities;
using SharpVNC.Client.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using static Dapper.SqlMapper;

namespace SharpVNC.Client.DataAccess
{
    /// <summary>
    /// Used to access the table and column information.
    /// </summary>
    public class RepositoryNamingProvider
    {
        /// <summary>
        /// Initialise a new instance of <see cref="RepositoryNamingProvider"/>.
        /// </summary>
        public RepositoryNamingProvider()
        {
            Tables = new Dictionary<Type, TableIdentifier>();

            List<AssemblyName> assemblyNames = Assembly.GetEntryAssembly().GetReferencedAssemblies().ToList();

            assemblyNames.Add(Assembly.GetEntryAssembly().GetName());

            foreach (AssemblyName assemblyName in assemblyNames)
            {
                Assembly assembly = Assembly.Load(assemblyName);

                IEnumerable<Type> types = assembly.GetTypes();

                foreach (Type type in types)
                {
                    TableAttribute tableAttribute = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), false).FirstOrDefault();

                    if (tableAttribute == null)
                    {
                        continue;
                    }

                    Dictionary<string, ColumnIdentifier> columns = new Dictionary<string, ColumnIdentifier>();

                    string tableName = tableAttribute.Name;

                    IEnumerable<PropertyInfo> properties = type.GetProperties();

                    foreach (PropertyInfo property in properties)
                    {
                        ColumnAttribute columnAttribute = (ColumnAttribute)property.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();

                        if (columnAttribute == null)
                        {
                            continue;
                        }

                        string columnName = columnAttribute.Name;

                        columns.Add(property.Name, new ColumnIdentifier(property, columnName, columnAttribute.Add, columnAttribute.Edit));
                    }

                    Tables.Add(type, new TableIdentifier(tableName, new ReadOnlyDictionary<string, ColumnIdentifier>(columns)));

                    ITypeMap mapper = (ITypeMap)Activator.CreateInstance(typeof(ColumnAttributeMapper<>).MakeGenericType(type), this);

                    SetTypeMap(type, mapper);
                }
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="TableIdentifier"/>.
        /// </summary>
        private Dictionary<Type, TableIdentifier> Tables { get; }

        /// <summary>
        /// Gets table information from the entity.
        /// </summary>
        /// <typeparam name="TEntityType">The type of entity.</typeparam>
        /// <returns>An instance of <see cref="TableIdentifier"/>.</returns>
        public TableIdentifier GetTable<TEntityType>()
            where TEntityType : class, IEntity
        {
            if (Tables.TryGetValue(typeof(TEntityType), out TableIdentifier table))
            {
                return table;
            }

            return null;
        }
    }

    /// <summary>
    /// Represents the table information of an entity.
    /// </summary>
    public class TableIdentifier
    {
        /// <summary>
        /// Initialise a new instance of <see cref="TableIdentifier"/>.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="columns">Collection of columns within this entity.</param>
        public TableIdentifier(string name, IDictionary<string, ColumnIdentifier> columns)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Columns = columns ?? throw new ArgumentNullException(nameof(columns));

            IDictionary<string, ColumnIdentifier> reversedColumns = new Dictionary<string, ColumnIdentifier>();

            foreach (string property in Columns.Keys)
            {
                ColumnIdentifier column = Columns[property];

                reversedColumns.Add(column.Name, column);
            }

            ReversedColumns = new ReadOnlyDictionary<string, ColumnIdentifier>(reversedColumns);
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a collection of columns within this entity that are identified by the property name.
        /// </summary>
        private IDictionary<string, ColumnIdentifier> Columns { get; }

        /// <summary>
        /// Gets a collection of columns within this entity that are identified by the column name.
        /// </summary>
        private IDictionary<string, ColumnIdentifier> ReversedColumns { get; }

        /// <summary>
        /// Gets column information from the property name.
        /// </summary>
        /// <param name="columnName">The name of the property.</param>
        /// <returns>An instance of <see cref="ColumnIdentifier"/>.</returns>
        public ColumnIdentifier GetColumnByPropertyName(string propertyName)
        {
            if (Columns.TryGetValue(propertyName, out ColumnIdentifier column))
            {
                return column;
            }

            return null;
        }

        /// <summary>
        /// Gets column information from the column name.
        /// </summary>
        /// <param name="columnName">The name of the column.</param>
        /// <returns>An instance of <see cref="ColumnIdentifier"/>.</returns>
        public ColumnIdentifier GetColumnByColumnName(string columnName)
        {
            if (ReversedColumns.TryGetValue(columnName, out ColumnIdentifier column))
            {
                return column;
            }

            return null;
        }

        /// <summary>
        /// Gets a collection of column information.
        /// </summary>
        /// <returns>A collection of column information.</returns>
        public IEnumerable<ColumnIdentifier> GetColumns()
        {
            return Columns.Select((column) => column.Value).ToList().AsReadOnly();
        }
    }

    /// <summary>
    /// Represents the column information of a column within an entity.
    /// </summary>
    public class ColumnIdentifier
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ColumnIdentifier"/>.
        /// </summary>
        /// <param name="property">The underlying property.</param>
        /// <param name="name">The name of the column.</param>
        /// <param name="add">Value indicating whether this data provided in this propert should be added to the database.</param>
        /// <param name="edit">Value indicating whether this data provided in this propert should be edited in the database.</param>
        public ColumnIdentifier(PropertyInfo property, string name, bool add, bool edit)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Add = add;
            Edit = edit;
        }

        /// <summary>
        /// Gets the underlying property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets a value indicating whether this data provided in this propert should be added to the database.
        /// </summary>
        public bool Add { get; }

        /// <summary>
        /// Gets a value indicating whether this data provided in this propert should be edited in the database.
        /// </summary>
        public bool Edit { get; }
    }

    /// <summary>
    /// Used to map SQL results into entity properties from columns.
    /// </summary>
    /// <typeparam name="TEntityType">The type of entity.</typeparam>
    public class ColumnAttributeMapper<TEntityType> : ITypeMap
        where TEntityType : class, IEntity
    {
        /// <summary>
        /// Initialise a new instance of <see cref="ColumnAttributeMapper"/>.
        /// </summary>
        /// <param name="repositoryNamingProvider">The repository naming provider.</param>
        public ColumnAttributeMapper(RepositoryNamingProvider repositoryNamingProvider)
        {
            RepositoryNamingProvider = repositoryNamingProvider ?? throw new ArgumentNullException(nameof(repositoryNamingProvider));

            List<ITypeMap> mappers = new List<ITypeMap>
            {
                new CustomPropertyTypeMap(typeof(TEntityType), (type, columnName) =>
                {
                    ColumnIdentifier column = RepositoryNamingProvider.GetTable<TEntityType>().GetColumnByColumnName(columnName);

                    if (column != null)
                    {
                        return column.Property;
                    }

                    return null;
                }),

                new DefaultTypeMap(typeof(TEntityType))
            };

            Mappers = mappers.AsReadOnly();
        }

        /// <summary>
        /// Gets the repository naming provider.
        /// </summary>
        private RepositoryNamingProvider RepositoryNamingProvider { get; }

        /// <summary>
        /// Gets a collection of <see cref="ITypeMap"/>.
        /// </summary>
        private IEnumerable<ITypeMap> Mappers { get; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="names"><inheritdoc/></param>
        /// <param name="types"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public ConstructorInfo FindConstructor(string[] names, Type[] types)
        {
            if (names is null)
            {
                throw new ArgumentNullException(nameof(names));
            }

            if (types is null)
            {
                throw new ArgumentNullException(nameof(types));
            }

            foreach (ITypeMap mapper in Mappers)
            {
                ConstructorInfo constructorInfo = mapper.FindConstructor(names, types);

                if (constructorInfo != null)
                {
                    return constructorInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public ConstructorInfo FindExplicitConstructor()
        {
            foreach (ITypeMap mapper in Mappers)
            {
                ConstructorInfo constructorInfo = mapper.FindExplicitConstructor();

                if (constructorInfo != null)
                {
                    return constructorInfo;
                }
            }

            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="constructor"><inheritdoc/></param>
        /// <param name="columnName"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public IMemberMap GetConstructorParameter(ConstructorInfo constructor, string columnName)
        {
            if (constructor is null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            if (columnName is null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            foreach (ITypeMap mapper in Mappers)
            {
                IMemberMap memberMap = mapper.GetConstructorParameter(constructor, columnName);

                if (memberMap != null)
                {
                    return memberMap;
                }
            }

            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="columnName"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public IMemberMap GetMember(string columnName)
        {
            if (columnName is null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            foreach (ITypeMap mapper in Mappers)
            {
                IMemberMap memberMap = mapper.GetMember(columnName);

                if (memberMap != null)
                {
                    return memberMap;
                }
            }

            return null;
        }
    }
}