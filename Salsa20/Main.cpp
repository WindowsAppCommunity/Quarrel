// Copyright (c) 2015 Nezametdinov E. Ildus
// See LICENSE.TXT for licensing details

//TODO: Replace read file code with byte[] (byte*) to istream
//TODO: Replace write file code with istream to byte[] (byte*)
//TODO: Place in referncable namespace to access from DiscordAPI in C#

#include "pch.h"
#include "Salsa20.h"

#include <iostream>
#include <cstdlib>
#include <fstream>
#include <string>

namespace Salsa20
{

	public ref class SalsaManager sealed {
	public:

		SalsaManager() : inputFileName_(), outputFileName_() {
			std::memset(key_, 0, sizeof(key_));
		}

		bool initialize(const Platform::Array<uint8_t>^ vectKey) {
			for (int i = 0; i < vectKey->Length; i++) {
				key_[i] = vectKey[i];
			}
			return true;
		}

		//To return and take 2 byte[]s



		void processFrame(Platform::Array<uint8_t>^* input) {

			//TODO: Handle nonce
			auto arraySize = (*input)->Length;

			const auto chunkSize = NUM_OF_BLOCKS_PER_CHUNK * Salsa20::BLOCK_SIZE;
			uint8_t chunk[chunkSize];

			//Initialize decrypter
			Salsa20 salsa20(key_);
			salsa20.setIv(&key_[IV_OFFSET]);

			//Split into chunks of chunkSize
			auto numChunks = arraySize / chunkSize;
			auto remainderSize = arraySize % chunkSize;

			//processChunks
			for (int i = 0; i < numChunks; i++) {
				//copy from input to chunk
				for (int pos = 0; pos < chunkSize; pos++) {
					chunk[pos] = (*input)->get(pos + (i*chunkSize));
				}
				//process
				salsa20.processBlocks(chunk, chunk, numChunks);
				//copy from chunk to input
				for (int pos = 0; pos < chunkSize; pos++) {
					(*input)->set(pos + (i*chunkSize), chunk[pos]);
				}
			}

			//processRemainder
			if (remainderSize != 0) {
				//copy from input to chunk
				for (int i = 0; i < remainderSize; i++) {
					chunk[i] = (*input)->get(i+(chunkSize*numChunks));
				}
				//process
				salsa20.processBytes(chunk, chunk, remainderSize);
				//copy from chunk to input
				for (int i = 0; i < remainderSize; i++) {
					(*input)->set(i + (chunkSize*numChunks), chunk[i]);
				}
			}
		}

		/*Array<byte>^ decodeFrame(Array<byte> data, Array<byte> nonce, int dataLength)
		{
			//memistream inputStream(data.begin(), dataLength);
			//memostream outputStream(new Array<byte>)
			
			const auto chunkSize = NUM_OF_BLOCKS_PER_CHUNK * Salsa20::BLOCK_SIZE;
			uint8_t chunk[chunkSize];

			// determine size of the file
			inputStream.seekg(0, std::ios_base::end);
			auto fileSize = inputStream.tellg();
			inputStream.seekg(0, std::ios_base::beg);

			// compute number of chunks and size of the remainder
			auto numChunks = fileSize / chunkSize;
			auto remainderSize = fileSize % chunkSize;

			// process file
			Salsa20 salsa20(key_);
			salsa20.setIv(&key_[IV_OFFSET]);

			for (decltype(numChunks) i = 0; i < numChunks; ++i)
			{
				inputStream.read(reinterpret_cast<char*>(chunk), sizeof(chunk));
				salsa20.processBlocks(chunk, chunk, NUM_OF_BLOCKS_PER_CHUNK);
				//outputStream.write(reinterpret_cast<const char*>(chunk), sizeof(chunk));

				float percentage = 100.0f * static_cast<float>(i + 1) / static_cast<float>(numChunks);
				std::printf("[%3.2f]\r", percentage);
			}

			if (remainderSize != 0)
			{
				inputStream.read(reinterpret_cast<char*>(chunk), remainderSize);
				salsa20.processBytes(chunk, chunk, remainderSize);
				//outputStream.write(reinterpret_cast<const char*>(chunk), remainderSize);
				std::cout << "[100.00]";
			}
		}
		*/

	private:
		/// Helper constants
		enum : size_t {
			NUM_OF_BLOCKS_PER_CHUNK = 8192,
			IV_OFFSET = Salsa20::KEY_SIZE,
			KEY_SIZE = Salsa20::KEY_SIZE + Salsa20::IV_SIZE
		};

		/**
		* \brief Reads byte from string.
		* \param[in] string string
		* \param[out] byte byte
		* \return true on success
		*/
		bool readByte(const wchar_t* string, uint8_t& byte) {
			byte = 0;

			for (uint32_t i = 0; i < 2; ++i) {
				uint8_t value = 0;
				char c = string[i];

				if (c >= '0' && c <= '9')
					value = c - '0';
				else if (c >= 'A' && c <= 'F')
					value = c - 'A' + 0x0A;
				else if (c >= 'a' && c <= 'f')
					value = c - 'a' + 0x0A;
				else
					return false;

				byte |= (value << (4 - i * 4));
			}

			return true;
		}

		/**
		* \brief Reads key from string.
		* \param[in] string string
		* \return true on success
		*/
		bool readKeyFromString(const std::wstring& string)
		{
			auto stringLength = string.length();

			if (stringLength != 2 * KEY_SIZE)
				return false;

			for (decltype(stringLength) i = 0; i < stringLength; i += 2)
			{
				if (!readByte(&string[i], key_[i / 2]))
					return false;
			}

			return true;
		}

		// Data members
		std::string inputFileName_, outputFileName_;
		uint8_t key_[KEY_SIZE];
	};
}
