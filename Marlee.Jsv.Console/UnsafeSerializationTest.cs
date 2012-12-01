using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace Marlee.Jsv.Console
{
  public class UnsafeSerializationTest
  {
    public class ByteBuffer
    {
      private byte[] _bytes;
      private int _position;
      private UTF8Encoding _encoding;
      private NumberFormatInfo _nfi;

      public ByteBuffer(int capacity)
      {
        _bytes = new byte[capacity];
        _position = 0;
        _encoding = new UTF8Encoding(false);
        _nfi = new NumberFormatInfo();
      }

      private void EnsureCapacity(int size)
      {
        if (_position + size > _bytes.Length)
        {
          var newBytes = new byte[2 * _bytes.Length];
          Array.Copy(_bytes, newBytes, _bytes.Length);
          _bytes = newBytes;
        }
      }

      public unsafe void Write(string s)
      {
        return;
        var count = _encoding.GetByteCount(s);

        EnsureCapacity(count);

        fixed (char* cPtr = s)
        fixed (byte* bPtr = _bytes)
        {
          _encoding.GetBytes(cPtr, s.Length, bPtr + _position, count);
        }
        _position += count;
      }

      public unsafe void Write(char c)
      {
        return;
        EnsureCapacity(2);

        var incr = 2;

        fixed (byte* bPtr = _bytes)
        {
          *((char*)(bPtr + _position)) = c;

          if (bPtr[_position + 1] == 0) // remove null terminator by incrementing only by 1
          {
            incr = 1;
          }
        }

        _position += incr;
      }

      public unsafe void Write(int i)
      {
        return;
        var s = i.ToString();

        var count = _encoding.GetByteCount(s);

        EnsureCapacity(count);

        fixed (char* cPtr = s)
        fixed (byte* bPtr = _bytes)
        {
          _encoding.GetBytes(cPtr, s.Length, bPtr + _position, count);
        }

        _position += count;
      }

      public void WriteToStream(Stream stream)
      {
        stream.Write(_bytes, 0, _position);
      }

      public override string ToString()
      {
        return _encoding.GetString(_bytes, 0, _position);
      }
    }

    public class CharBuffer
    {
      private char[] _chars;
      private int _position;
      private Encoding _encoding;
      private NumberFormatInfo _nfi;

      public CharBuffer(int capacity)
      {
        _chars = new char[capacity];
        _position = 0;
        _encoding = Encoding.UTF8;
        _nfi = new NumberFormatInfo();
      }

      private void EnsureCapacity(int size)
      {
        if (_position + size > _chars.Length)
        {
          var newBytes = new char[2 * _chars.Length];
          Array.Copy(_chars, newBytes, _chars.Length);
          _chars = newBytes;
        }
      }

      public void Write(string s)
      {
        //return;
        EnsureCapacity(s.Length);

        s.CopyTo(0, _chars, _position, s.Length);

        _position += s.Length;
      }

      public void Write(char c)
      {
        //return;
        EnsureCapacity(1);

        _chars[_position] = c;

        ++_position;
      }

      public void Write(int i)
      {
        var s = i.ToString(_nfi);

        Write(s);
      }

      public void WriteToStream(Stream stream)
      {
        var bytes = _encoding.GetBytes(_chars, 0, _position);
        stream.Write(bytes, 0, _position);
      }

      public override string ToString()
      {
        var bytes = _encoding.GetBytes(_chars, 0, _position);
        return _encoding.GetString(bytes, 0, _position);
      }
    }

    internal static void SerializeCustomerBytes(Marlee.Jsv.Console.Program.Customer c, Stream s)
    {
      //var byteBuffer = new ByteBuffer(256);
      //byteBuffer.WriteInt32(4);
      //byteBuffer.WriteChar('{');
      //byteBuffer.WriteString("test");
      //var s = byteBuffer.ToString();

      var byteBuffer = new CharBuffer(256);

      SerializeCustomer(byteBuffer, c);

      byteBuffer.WriteToStream(s);
      //var x = byteBuffer.ToString();
    }

    //private static void WriteString(ByteBuffer b, string s)
    //{
    //  b.Write('"');
    //  if (s.IndexOf('\"') >= 0)
    //  {
    //    b.Write(s.Replace("\"", "\"\""));
    //  }
    //  else
    //  {
    //    b.Write(s);
    //  }
    //  b.Write('"');
    //}

    //private static void SerializeCustomer(ByteBuffer b, Marlee.Jsv.Console.Program.Customer c)
    //{
    //  b.Write('{');
    //  if (c.Name != null)
    //  {
    //    b.Write("Name:");
    //    WriteString(b, c.Name);
    //    b.Write(',');
    //  }
    //  b.Write("ID:");
    //  b.Write(c.ID);
    //  b.Write(',');

    //  if (c.Roles != null)
    //  {
    //    b.Write("Roles:[");
    //    for (var i = 0; i < c.Roles.Count; i++)
    //    {
    //      WriteString(b, c.Roles[i]);
    //      b.Write(']');
    //    }
    //    b.Write(']');
    //    b.Write(',');
    //  }

    //  if (c.Address != null)
    //  {
    //    SerializeAddress(b, c.Address);
    //    b.Write(',');
    //  }

    //  if (c.PhoneNumbers != null)
    //  {
    //    b.Write("PhoneNumbers:[");
    //    for (var i = 0; i < c.PhoneNumbers.Count; i++)
    //    {
    //      SerializePhoneNumber(b, c.PhoneNumbers[i]);
    //    }
    //    b.Write(']');
    //    b.Write(',');
    //  }

    //  if (c.FirstName != null)
    //  {
    //    b.Write("FirstName:");
    //    WriteString(b, c.FirstName);
    //    b.Write(',');
    //  }

    //  if (c.LastName != null)
    //  {
    //    b.Write("LastName:");
    //    WriteString(b, c.LastName);
    //    b.Write(',');
    //  }

    //  b.Write("Age:");
    //  b.Write(c.Age);
    //  b.Write('}');
    
    //}

    //private static void SerializeAddress(ByteBuffer b, Marlee.Jsv.Console.Program.Address a)
    //{
    //  b.Write('{');
    //  if (a.Street != null)
    //  {
    //    b.Write("Street:");
    //    WriteString(b, a.Street);
    //    b.Write(',');
    //  }

    //  b.Write("ID:");
    //  b.Write(a.ID);
    //  b.Write('}');
    //}

    //private static void SerializePhoneNumber(ByteBuffer b, Marlee.Jsv.Console.Program.PhoneNumber p)
    //{
    //  b.Write('{');
    //  if (p.Number != null)
    //  {
    //    b.Write("Number:");
    //    WriteString(b, p.Number);
    //    b.Write(',');
    //  }

    //  b.Write("Type:");

    //  switch (p.Type)
    //  {
    //    case Marlee.Jsv.Console.Program.PhoneNumberTypes.Land:
    //      b.Write("Land");
    //      break;
    //    case Marlee.Jsv.Console.Program.PhoneNumberTypes.Mobile:
    //      b.Write("Mobile");
    //      break;
    //  }
    //  b.Write('}');
    //}


    private static void WriteString(CharBuffer b, string s)
    {
      b.Write('"');
      if (s.IndexOf('\"') >= 0)
      {
        b.Write(s.Replace("\"", "\"\""));
      }
      else
      {
        b.Write(s);
      }
      b.Write('"');
    }

    private static void SerializeCustomer(CharBuffer b, Marlee.Jsv.Console.Program.Customer c)
    {
      b.Write('{');
      if (c.Name != null)
      {
        b.Write("Name:");
        WriteString(b, c.Name);
        b.Write(',');
      }
      b.Write("ID:");
      b.Write(c.ID);
      b.Write(',');

      if (c.Roles != null)
      {
        b.Write("Roles:[");
        for (var i = 0; i < c.Roles.Count; i++)
        {
          WriteString(b, c.Roles[i]);
          b.Write(']');
        }
        b.Write(']');
        b.Write(',');
      }

      if (c.Address != null)
      {
        SerializeAddress(b, c.Address);
        b.Write(',');
      }

      if (c.PhoneNumbers != null)
      {
        b.Write("PhoneNumbers:[");
        for (var i = 0; i < c.PhoneNumbers.Count; i++)
        {
          SerializePhoneNumber(b, c.PhoneNumbers[i]);
        }
        b.Write(']');
        b.Write(',');
      }

      if (c.FirstName != null)
      {
        b.Write("FirstName:");
        WriteString(b, c.FirstName);
        b.Write(',');
      }

      if (c.LastName != null)
      {
        b.Write("LastName:");
        WriteString(b, c.LastName);
        b.Write(',');
      }

      b.Write("Age:");
      b.Write(c.Age);
      b.Write('}');

    }

    private static void SerializeAddress(CharBuffer b, Marlee.Jsv.Console.Program.Address a)
    {
      b.Write('{');
      if (a.Street != null)
      {
        b.Write("Street:");
        WriteString(b, a.Street);
        b.Write(',');
      }

      b.Write("ID:");
      b.Write(a.ID);
      b.Write('}');
    }

    private static void SerializePhoneNumber(CharBuffer b, Marlee.Jsv.Console.Program.PhoneNumber p)
    {
      b.Write('{');
      if (p.Number != null)
      {
        b.Write("Number:");
        WriteString(b, p.Number);
        b.Write(',');
      }

      b.Write("Type:");

      switch (p.Type)
      {
        case Marlee.Jsv.Console.Program.PhoneNumberTypes.Land:
          b.Write("Land");
          break;
        case Marlee.Jsv.Console.Program.PhoneNumberTypes.Mobile:
          b.Write("Mobile");
          break;
      }
      b.Write('}');
    }


  }
}
