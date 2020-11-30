using EM.GIS.Data;

namespace EM.GIS.Gdals
{
    internal class GdalFieldDfn : IFieldDefn
    {
        public OSGeo.OGR.FieldDefn FieldDefn { get; set; }

        public GdalFieldDfn(OSGeo.OGR.FieldDefn fieldDefn)
        {
            FieldDefn = fieldDefn;
        }

        public string Name
        {
            get
            {
                return FieldDefn?.GetName();
            }
            set
            {
                if (FieldDefn != null)
                {
                    FieldDefn.SetName(value);
                }
            }
        }
        public FieldType FieldType
        {
            get
            {
                var value = FieldType.Unkown;
                if (FieldDefn != null)
                {
                    value = FieldDefn.GetFieldType().ToFieldType();
                }
                return value;
            }
            set
            {
                if (FieldDefn != null)
                {
                    if (value != FieldType.Unkown)
                    {
                        FieldDefn.SetType(value.ToFieldType());
                    }
                }
            }
        }
        public int Length
        {
            get
            {
                var value = 0;
                if (FieldDefn != null)
                {
                    value = FieldDefn.GetWidth();
                }
                return value;
            }
            set
            {
                if (FieldDefn != null)
                {
                    FieldDefn.SetWidth(value);
                }
            }
        }
        public int Precision
        {
            get
            {
                var value = 0;
                if (FieldDefn != null)
                {
                    value = FieldDefn.GetPrecision();
                }
                return value;
            }
            set
            {
                if (FieldDefn != null)
                {
                    FieldDefn.SetPrecision(value);
                }
            }
        }
        public bool IsIgnored
        {
            get
            {
                var value = false;
                if (FieldDefn != null)
                {
                    value = FieldDefn.IsIgnored() == 1;
                }
                return value;
            }
            set
            {
                if (FieldDefn != null)
                {
                    FieldDefn.SetIgnored(value ? 1 : 0);
                }
            }
        }
        public bool IsNullable
        {
            get
            {
                var value = false;
                if (FieldDefn != null)
                {
                    value = FieldDefn.IsNullable() == 1;
                }
                return value;
            }
            set
            {
                if (FieldDefn != null)
                {
                    FieldDefn.SetNullable(value ? 1 : 0);
                }
            }
        }

    }
}