using System;
using System.Runtime.InteropServices;

namespace Mono.Security.X509
{
	internal class OSX509Certificates
	{
		public enum SecTrustResult
		{
			Invalid = 0,
			Proceed = 1,
			Confirm = 2,
			Deny = 3,
			Unspecified = 4,
			RecoverableTrustFailure = 5,
			FatalTrustFailure = 6,
			ResultOtherError = 7
		}

		public const string SecurityLibrary = "/System/Library/Frameworks/Security.framework/Security";

		public const string CoreFoundationLibrary = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

		private static IntPtr sslsecpolicy = SecPolicyCreateSSL(0, IntPtr.Zero);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecCertificateCreateWithData(IntPtr allocator, IntPtr nsdataRef);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern int SecTrustCreateWithCertificates(IntPtr certOrCertArray, IntPtr policies, out IntPtr sectrustref);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern IntPtr SecPolicyCreateSSL(int server, IntPtr cfStringHostname);

		[DllImport("/System/Library/Frameworks/Security.framework/Security")]
		private static extern int SecTrustEvaluate(IntPtr secTrustRef, out SecTrustResult secTrustResultTime);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private unsafe static extern IntPtr CFDataCreate(IntPtr allocator, byte* bytes, IntPtr length);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern void CFRelease(IntPtr handle);

		[DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
		private static extern IntPtr CFArrayCreate(IntPtr allocator, IntPtr values, IntPtr numValues, IntPtr callbacks);

		private unsafe static IntPtr MakeCFData(byte[] data)
		{
			fixed (byte* bytes = &data[0])
			{
				return CFDataCreate(IntPtr.Zero, bytes, (IntPtr)data.Length);
			}
		}

		private unsafe static IntPtr FromIntPtrs(IntPtr[] values)
		{
			//IL_0015->IL001c: Incompatible stack types: I vs Ref
			fixed (IntPtr* ptr = &(values != null && values.Length != 0 ? ref values[0] : ref *(IntPtr*)null))
			{
				return CFArrayCreate(IntPtr.Zero, (IntPtr)ptr, (IntPtr)values.Length, IntPtr.Zero);
			}
		}

		public static SecTrustResult TrustEvaluateSsl(X509CertificateCollection certificates)
		{
			try
			{
				return _TrustEvaluateSsl(certificates);
			}
			catch
			{
				return SecTrustResult.Deny;
			}
		}

		private static SecTrustResult _TrustEvaluateSsl(X509CertificateCollection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			int count = certificates.Count;
			IntPtr[] array = new IntPtr[count];
			IntPtr[] array2 = new IntPtr[count];
			IntPtr intPtr = IntPtr.Zero;
			try
			{
				for (int i = 0; i < count; i++)
				{
					array[i] = MakeCFData(certificates[i].RawData);
				}
				for (int j = 0; j < count; j++)
				{
					array2[j] = SecCertificateCreateWithData(IntPtr.Zero, array[j]);
					if (array2[j] == IntPtr.Zero)
					{
						return SecTrustResult.Deny;
					}
				}
				intPtr = FromIntPtrs(array2);
				IntPtr sectrustref;
				if (SecTrustCreateWithCertificates(intPtr, sslsecpolicy, out sectrustref) == 0)
				{
					SecTrustResult secTrustResultTime;
					if (SecTrustEvaluate(sectrustref, out secTrustResultTime) != 0)
					{
						return SecTrustResult.Deny;
					}
					CFRelease(sectrustref);
					return secTrustResultTime;
				}
				return SecTrustResult.Deny;
			}
			finally
			{
				for (int k = 0; k < count; k++)
				{
					if (array[k] != IntPtr.Zero)
					{
						CFRelease(array[k]);
					}
				}
				if (intPtr != IntPtr.Zero)
				{
					CFRelease(intPtr);
				}
				else
				{
					for (int l = 0; l < count; l++)
					{
						if (array2[l] != IntPtr.Zero)
						{
							CFRelease(array2[l]);
						}
					}
				}
			}
		}
	}
}
