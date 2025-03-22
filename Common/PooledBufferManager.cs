using System.Buffers;

namespace Common
{
    public static class PooledBufferManager
    {
        private static ArrayPool<byte> buffer_pool_ = ArrayPool<byte>.Create();

		public static byte[] RentBuffer(int size)
		{
			return buffer_pool_.Rent(size);
		}

		public static void ReturnBuffer(byte[] buffer)
		{
			buffer_pool_.Return(buffer);
		}
    }
}