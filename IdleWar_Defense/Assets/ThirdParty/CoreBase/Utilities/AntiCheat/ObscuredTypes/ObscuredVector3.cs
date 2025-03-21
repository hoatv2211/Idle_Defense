﻿using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Utilities.AntiCheat
{
    /// <summary>
    /// Use it instead of regular <c>Vector3</c> for any cheating-sensitive variables.
    /// </summary>
    /// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
    /// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
    [Serializable]
    public struct ObscuredVector3
    {
        private static int cryptoKey = 120207;
        private static readonly Vector3 zero = Vector3.zero;

#if UNITY_EDITOR
        // For internal Editor usage only (may be useful for drawers).
        public static int cryptoKeyEditor = cryptoKey;
#endif

        [SerializeField]
        private int currentCryptoKey;

        [SerializeField]
        private RawEncryptedVector3 hiddenValue;

        [SerializeField]
        private bool inited;

        private ObscuredVector3(Vector3 value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(value);

            inited = true;
        }

        /// <summary>
        /// Mimics constructor of regular Vector3.
        /// </summary>
        /// <param name="x">X component of the vector</param>
        /// <param name="y">Y component of the vector</param>
        /// <param name="z">Z component of the vector</param>
        public ObscuredVector3(float x, float y, float z)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(x, y, z, currentCryptoKey);

            inited = true;
        }

        public float x
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.x);
                return decrypted;
            }

            set
            {
                hiddenValue.x = InternalEncryptField(value);
            }
        }

        public float y
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.y);
                return decrypted;
            }

            set
            {
                hiddenValue.y = InternalEncryptField(value);
            }
        }

        public float z
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.z);
                return decrypted;
            }

            set
            {
                hiddenValue.z = InternalEncryptField(value);
            }
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid ObscuredVector3 index!");
                }
            }
        }

        /// <summary>
        /// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
        /// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
        /// </summary>
        public static void SetNewCryptoKey(int newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// Use this simple encryption method to encrypt any Vector3 value, uses default crypto key.
        /// </summary>
        public static RawEncryptedVector3 Encrypt(Vector3 value)
        {
            return Encrypt(value, 0);
        }

        /// <summary>
        /// Use this simple encryption method to encrypt any Vector3 value, uses passed crypto key.
        /// </summary>
        public static RawEncryptedVector3 Encrypt(Vector3 value, int key)
        {
            return Encrypt(value.x, value.y, value.z, key);
        }

        /// <summary>
        /// Use this simple encryption method to encrypt Vector3 components, uses passed crypto key.
        /// </summary>
        public static RawEncryptedVector3 Encrypt(float x, float y, float z, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            RawEncryptedVector3 result;
            result.x = ObscuredFloat.Encrypt(x, key);
            result.y = ObscuredFloat.Encrypt(y, key);
            result.z = ObscuredFloat.Encrypt(z, key);

            return result;
        }

        /// <summary>
        /// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses default crypto key.
        /// </summary>
        public static Vector3 Decrypt(RawEncryptedVector3 value)
        {
            return Decrypt(value, 0);
        }

        /// <summary>
        /// Use it to decrypt RawEncryptedVector3 you got from Encrypt(), uses passed crypto key.
        /// </summary>
        public static Vector3 Decrypt(RawEncryptedVector3 value, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            Vector3 result;
            result.x = ObscuredFloat.Decrypt(value.x, key);
            result.y = ObscuredFloat.Decrypt(value.y, key);
            result.z = ObscuredFloat.Decrypt(value.z, key);

            return result;
        }

        /// <summary>
        /// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
        /// </summary>
        public void ApplyNewCryptoKey()
        {
            if (currentCryptoKey != cryptoKey)
            {
                hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
                currentCryptoKey = cryptoKey;
            }
        }

        /// <summary>
        /// Allows to change current crypto key to the new random value and re-encrypt variable using it.
        /// Use it for extra protection against 'unknown value' search.
        /// Just call it sometimes when your variable doesn't change to fool the cheater.
        /// </summary>
        public void RandomizeCryptoKey()
        {
            var decrypted = InternalDecrypt();

            do
            {
                currentCryptoKey = Random.Range(int.MinValue, int.MaxValue);
            } while (currentCryptoKey == 0);
            hiddenValue = Encrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// Allows to pick current obscured value as is.
        /// </summary>
        /// Use it in conjunction with SetEncrypted().<br/>
        /// Useful for saving data in obscured state.
        public RawEncryptedVector3 GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// Allows to explicitly set current obscured value.
        /// </summary>
        /// Use it in conjunction with GetEncrypted().<br/>
        /// Useful for loading data stored in obscured state.
        public void SetEncrypted(RawEncryptedVector3 encrypted)
        {
            inited = true;
            hiddenValue = encrypted;

            if (currentCryptoKey == 0)
            {
                currentCryptoKey = cryptoKey;
            }
        }

        /// <summary>
        /// Alternative to the type cast, use if you wish to get decrypted value 
        /// but can't or don't want to use cast to the regular type.
        /// </summary>
        /// <returns>Decrypted value.</returns>
        public Vector3 GetDecrypted()
        {
            return InternalDecrypt();
        }

        private Vector3 InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = Encrypt(zero, cryptoKey);
                inited = true;

                return zero;
            }

            Vector3 value;

            value.x = ObscuredFloat.Decrypt(hiddenValue.x, currentCryptoKey);
            value.y = ObscuredFloat.Decrypt(hiddenValue.y, currentCryptoKey);
            value.z = ObscuredFloat.Decrypt(hiddenValue.z, currentCryptoKey);

            return value;
        }

        private float InternalDecryptField(int encrypted)
        {
            var key = cryptoKey;

            if (currentCryptoKey != cryptoKey)
            {
                key = currentCryptoKey;
            }

            var result = ObscuredFloat.Decrypt(encrypted, key);
            return result;
        }

        private int InternalEncryptField(float encrypted)
        {
            var result = ObscuredFloat.Encrypt(encrypted, cryptoKey);
            return result;
        }

        #region operators, overrides, etc.
        //! @cond
        public static implicit operator ObscuredVector3(Vector3 value)
        {
            return new ObscuredVector3(value);
        }

        public static implicit operator Vector3(ObscuredVector3 value)
        {
            return value.InternalDecrypt();
        }

        public static ObscuredVector3 operator +(ObscuredVector3 a, ObscuredVector3 b)
        {
            return a.InternalDecrypt() + b.InternalDecrypt();
        }

        public static ObscuredVector3 operator +(Vector3 a, ObscuredVector3 b)
        {
            return a + b.InternalDecrypt();
        }

        public static ObscuredVector3 operator +(ObscuredVector3 a, Vector3 b)
        {
            return a.InternalDecrypt() + b;
        }

        public static ObscuredVector3 operator -(ObscuredVector3 a, ObscuredVector3 b)
        {
            return a.InternalDecrypt() - b.InternalDecrypt();
        }

        public static ObscuredVector3 operator -(Vector3 a, ObscuredVector3 b)
        {
            return a - b.InternalDecrypt();
        }

        public static ObscuredVector3 operator -(ObscuredVector3 a, Vector3 b)
        {
            return a.InternalDecrypt() - b;
        }

        public static ObscuredVector3 operator -(ObscuredVector3 a)
        {
            return -a.InternalDecrypt();
        }

        public static ObscuredVector3 operator *(ObscuredVector3 a, float d)
        {
            return a.InternalDecrypt() * d;
        }

        public static ObscuredVector3 operator *(float d, ObscuredVector3 a)
        {
            return d * a.InternalDecrypt();
        }

        public static ObscuredVector3 operator /(ObscuredVector3 a, float d)
        {
            return a.InternalDecrypt() / d;
        }

        public static bool operator ==(ObscuredVector3 lhs, ObscuredVector3 rhs)
        {
            return lhs.InternalDecrypt() == rhs.InternalDecrypt();
        }

        public static bool operator ==(Vector3 lhs, ObscuredVector3 rhs)
        {
            return lhs == rhs.InternalDecrypt();
        }

        public static bool operator ==(ObscuredVector3 lhs, Vector3 rhs)
        {
            return lhs.InternalDecrypt() == rhs;
        }

        public static bool operator !=(ObscuredVector3 lhs, ObscuredVector3 rhs)
        {
            return lhs.InternalDecrypt() != rhs.InternalDecrypt();
        }

        public static bool operator !=(Vector3 lhs, ObscuredVector3 rhs)
        {
            return lhs != rhs.InternalDecrypt();
        }

        public static bool operator !=(ObscuredVector3 lhs, Vector3 rhs)
        {
            return lhs.InternalDecrypt() != rhs;
        }

        public override bool Equals(object other)
        {
            return InternalDecrypt().Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return InternalDecrypt().GetHashCode();
        }

        /// <summary>
        /// Returns a nicely formatted string for this vector.
        /// </summary>
        public override string ToString()
        {
            return InternalDecrypt().ToString();
        }

        /// <summary>
        /// Returns a nicely formatted string for this vector.
        /// </summary>
        public string ToString(string format)
        {
            return InternalDecrypt().ToString(format);
        }

        //! @endcond
        #endregion

        /// <summary>
        /// Used to store encrypted Vector3.
        /// </summary>
        [Serializable]
        public struct RawEncryptedVector3
        {
            /// <summary>
            /// Encrypted value
            /// </summary>
            public int x;

            /// <summary>
            /// Encrypted value
            /// </summary>
            public int y;

            /// <summary>
            /// Encrypted value
            /// </summary>
            public int z;
        }
    }
}