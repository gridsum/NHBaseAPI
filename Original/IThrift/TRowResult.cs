/**
 * Autogenerated by Thrift Compiler (0.9.2)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;


/// <summary>
/// Holds row name and then a map of columns to cells.
/// </summary>
#if !SILVERLIGHT
[Serializable]
#endif
public partial class TRowResult : TBase
{
  private byte[] _row;
  private Dictionary<byte[], TCell> _columns;
  private List<TColumn> _sortedColumns;

  public byte[] Row
  {
    get
    {
      return _row;
    }
    set
    {
      __isset.row = true;
      this._row = value;
    }
  }

  public Dictionary<byte[], TCell> Columns
  {
    get
    {
      return _columns;
    }
    set
    {
      __isset.columns = true;
      this._columns = value;
    }
  }

  public List<TColumn> SortedColumns
  {
    get
    {
      return _sortedColumns;
    }
    set
    {
      __isset.sortedColumns = true;
      this._sortedColumns = value;
    }
  }


  public Isset __isset;
  #if !SILVERLIGHT
  [Serializable]
  #endif
  public struct Isset {
    public bool row;
    public bool columns;
    public bool sortedColumns;
  }

  public TRowResult() {
  }

  public void Read (TProtocol iprot)
  {
    TField field;
    iprot.ReadStructBegin();
    while (true)
    {
      field = iprot.ReadFieldBegin();
      if (field.Type == TType.Stop) { 
        break;
      }
      switch (field.ID)
      {
        case 1:
          if (field.Type == TType.String) {
            Row = iprot.ReadBinary();
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 2:
          if (field.Type == TType.Map) {
            {
              Columns = new Dictionary<byte[], TCell>();
              TMap _map4 = iprot.ReadMapBegin();
              for( int _i5 = 0; _i5 < _map4.Count; ++_i5)
              {
                byte[] _key6;
                TCell _val7;
                _key6 = iprot.ReadBinary();
                _val7 = new TCell();
                _val7.Read(iprot);
                Columns[_key6] = _val7;
              }
              iprot.ReadMapEnd();
            }
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        case 3:
          if (field.Type == TType.List) {
            {
              SortedColumns = new List<TColumn>();
              TList _list8 = iprot.ReadListBegin();
              for( int _i9 = 0; _i9 < _list8.Count; ++_i9)
              {
                TColumn _elem10;
                _elem10 = new TColumn();
                _elem10.Read(iprot);
                SortedColumns.Add(_elem10);
              }
              iprot.ReadListEnd();
            }
          } else { 
            TProtocolUtil.Skip(iprot, field.Type);
          }
          break;
        default: 
          TProtocolUtil.Skip(iprot, field.Type);
          break;
      }
      iprot.ReadFieldEnd();
    }
    iprot.ReadStructEnd();
  }

  public void Write(TProtocol oprot) {
    TStruct struc = new TStruct("TRowResult");
    oprot.WriteStructBegin(struc);
    TField field = new TField();
    if (Row != null && __isset.row) {
      field.Name = "row";
      field.Type = TType.String;
      field.ID = 1;
      oprot.WriteFieldBegin(field);
      oprot.WriteBinary(Row);
      oprot.WriteFieldEnd();
    }
    if (Columns != null && __isset.columns) {
      field.Name = "columns";
      field.Type = TType.Map;
      field.ID = 2;
      oprot.WriteFieldBegin(field);
      {
        oprot.WriteMapBegin(new TMap(TType.String, TType.Struct, Columns.Count));
        foreach (byte[] _iter11 in Columns.Keys)
        {
          oprot.WriteBinary(_iter11);
          Columns[_iter11].Write(oprot);
        }
        oprot.WriteMapEnd();
      }
      oprot.WriteFieldEnd();
    }
    if (SortedColumns != null && __isset.sortedColumns) {
      field.Name = "sortedColumns";
      field.Type = TType.List;
      field.ID = 3;
      oprot.WriteFieldBegin(field);
      {
        oprot.WriteListBegin(new TList(TType.Struct, SortedColumns.Count));
        foreach (TColumn _iter12 in SortedColumns)
        {
          _iter12.Write(oprot);
        }
        oprot.WriteListEnd();
      }
      oprot.WriteFieldEnd();
    }
    oprot.WriteFieldStop();
    oprot.WriteStructEnd();
  }

  public override string ToString() {
    StringBuilder __sb = new StringBuilder("TRowResult(");
    bool __first = true;
    if (Row != null && __isset.row) {
      if(!__first) { __sb.Append(", "); }
      __first = false;
      __sb.Append("Row: ");
      __sb.Append(Row);
    }
    if (Columns != null && __isset.columns) {
      if(!__first) { __sb.Append(", "); }
      __first = false;
      __sb.Append("Columns: ");
      __sb.Append(Columns);
    }
    if (SortedColumns != null && __isset.sortedColumns) {
      if(!__first) { __sb.Append(", "); }
      __first = false;
      __sb.Append("SortedColumns: ");
      __sb.Append(SortedColumns);
    }
    __sb.Append(")");
    return __sb.ToString();
  }

}

