using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Marlee.Internal
{
  public interface IKnownTypeProvider
  {
    Delegate TryGetKnownTypeDelegate(Type type);
  }
}
