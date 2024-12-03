#include <ETH.h>
#include <WiFi.h>
#include <WiFiAP.h>
#include <WiFiClient.h>
#include <WiFiGeneric.h>
#include <WiFiMulti.h>
#include <WiFiScan.h>
#include <WiFiServer.h>
#include <WiFiSTA.h>
#include <WiFiType.h>
#include <WiFiUdp.h>

#include <FastLED.h>

#define LED_PIN     13
#define NUM_LEDS    128
#define BRIGHTNESS  16
#define LED_TYPE    WS2812B
#define COLOR_ORDER GRB
CRGB leds[NUM_LEDS];

#define UPDATES_PER_SECOND 100

char ssid[] = "";     //  your network SSID (name)
char password[] = "";  // your network password
int status = WL_IDLE_STATUS;     // the Wifi radio's status

WiFiUDP Udp;
unsigned int localPort = 2390;      // local port to listen on
char packetBuffer[255]; //buffer to hold incoming packet
unsigned char  ReplyBuffer[] = "ack";       // a string to send back

char StatusValues[NUM_LEDS];

bool DataReady = true;

unsigned long update_delay = 100;
unsigned long update_previousMillis = 0;

void setup() {
  delay( 3000 ); // power-up safety delay
  FastLED.addLeds<LED_TYPE, LED_PIN, COLOR_ORDER>(leds, NUM_LEDS).setCorrection( TypicalLEDStrip );
  FastLED.setBrightness(  BRIGHTNESS );

  Serial.begin(115200);  // start serial for output
  delay(1); //Keep watchdog happy
  Serial.println("Startup:");

  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(25);
    Serial.print(".");
  }

  Serial.println("!");
  Serial.println(WiFi.localIP());
  
  Serial.println("\nStarting connection to server...");
  // if you get a connection, report back via serial:
  Udp.begin(localPort);
}

void loop() {
    // if there's data available, read a packet
  int packetSize = Udp.parsePacket();
  if (packetSize) {
    Serial.print("Received packet of size ");
    Serial.println(packetSize);
    Serial.print("From ");
    IPAddress remoteIp = Udp.remoteIP();
    Serial.print(remoteIp);
    Serial.print(", port ");
    Serial.println(Udp.remotePort());

    // read the packet into packetBufffer
    int len = Udp.read(packetBuffer, 255);
    if (len > 0) {
      packetBuffer[len] = 0;
    }
    Serial.println("Contents:");
    Serial.println(packetBuffer);

    // send a reply, to the IP address and port that sent us the packet we received
    Udp.beginPacket(Udp.remoteIP(), Udp.remotePort());
    Udp.write(ReplyBuffer, 3);
    Udp.endPacket();

    if(len >= NUM_LEDS)
    {
      for(int i = 0; i < NUM_LEDS; i++)
      {
        StatusValues[i] = 0;
      }
      
      //Parse out the status values
      for(int i = 0; i < NUM_LEDS; i++)
      {
        StatusValues[i] = packetBuffer[i];
      }
      DataReady = true;
    }
  }

  unsigned long currentMillis = millis();
  
  if(DataReady || currentMillis - update_previousMillis >= update_delay)
  {
    update_previousMillis = currentMillis;

    for(int i = 0; i < NUM_LEDS; i++)
    {      
      if(StatusValues[i] == 'B') {leds[i] = CRGB::Red;}
      else if(StatusValues[i] == 'A') {leds[i] = CRGB::Green;}
      else if(StatusValues[i] == 'Y') {leds[i] = CRGB::Yellow;}
      else {leds[i] = CRGB::Black;}
    }
    
    addGlitter(80);
    
    FastLED.show();
    DataReady = false;
  }
}


void addGlitter( fract8 chanceOfGlitter) 
{
  if( random8() < chanceOfGlitter) {
    leds[ random16(NUM_LEDS) ] += CRGB::Gold;
  }
}
