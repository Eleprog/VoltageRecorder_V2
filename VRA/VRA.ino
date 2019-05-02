struct DateTime
{
	byte
		year = 18,
		month = 12,
		day = 23,
		hour = 12,
		minute = 35,
		second = 0;
	int	milliseconds = 0;
};

uint32_t currTime;
uint32_t prevTime = 0;
int countSeconds = 0;
typedef struct
{
	byte* code;
}ByteAnalog;

ByteAnalog byteAnalog[6];
byte packeg[21];
DateTime _dateTime;


void incrementDateTime(struct DateTime &dateTime) {
	dateTime.milliseconds += 10;
	if (dateTime.milliseconds >= 1000)
	{
		dateTime.milliseconds = 0;
		dateTime.second++;
	}
	if (dateTime.second >= 60)
	{
		dateTime.second = 0;
		dateTime.minute++;
	}
	if (dateTime.minute >= 60)
	{
		dateTime.minute = 0;
		dateTime.hour++;
	}
	if (dateTime.hour >= 24) {
		dateTime.hour = 0;
	}

}

byte* intTobyte(const int &value) {
	static byte b[2];
	b[0] = value % 100;
	b[1] = value / 100;

	return b;
}

int byteToInt(byte* bytes) {
	return bytes[0] + bytes[1] * 100;
}
int byteToInt2(byte first, byte second) {
	return first + second * 100;
}

void setup() {
	for (byte i = 0; i < 6; i++)
	{
		pinMode(i + 14, INPUT);
	}
	Serial.begin(115200);
}

void loop() {
	currTime = millis();
	if (currTime - prevTime >= 10)
	{
		prevTime = currTime;
		incrementDateTime(_dateTime);

		for (byte i = 0; i < 6; i++)
		{
			int a = analogRead(i + 14);
			byte* b = intTobyte(random(0+i*30, 100+i * 30));
			byteAnalog[i].code = b;
		}
		byte* byteMillis = intTobyte(_dateTime.milliseconds);
		packeg[0] = 255;
		packeg[1] = byteMillis[0];
		packeg[2] = byteMillis[1];
		packeg[3] = _dateTime.second;
		packeg[4] = _dateTime.minute;
		packeg[5] = _dateTime.hour;
		packeg[6] = _dateTime.day;
		packeg[7] = _dateTime.month;
		packeg[8] = _dateTime.year;
		for (byte i = 0; i < 6; i++)
		{
			packeg[i * 2 + 9] = byteAnalog[i].code[0];
			packeg[i * 2 + 10] = byteAnalog[i].code[1];
		}
		String pack;
		for (byte i = 0; i < sizeof(packeg); i++)
		{
			pack += packeg[i];
			pack += ",";
		}
		pack += packeg[0]; pack += " ";
		pack += byteToInt2(packeg[1], packeg[2]); pack += " ";
		pack += packeg[3]; pack += " ";
		pack += packeg[4]; pack += " ";
		pack += packeg[5]; pack += " ";
		pack += packeg[6]; pack += " ";
		pack += packeg[7]; pack += " ";
		pack += packeg[8]; pack += " ";

		
		Serial.write(packeg, 21);
		//Serial.println(pack);

		//if (_dateTime.milliseconds>=50) Serial.end();
	}

}

