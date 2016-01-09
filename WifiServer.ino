#include <SPI.h>
#include <WiFi.h>
#include <Wire.h>
#include <SPI.h>

//Arduino PWM Speed Control：
int E1 = 4;  
int M1 = 5; 
int E2 = 7;                      
int M2 = 6; 

char ssid[] = "your network SSID";          //  your network SSID (name) 
char pass[] = "your network password";   // your network password
int status = WL_IDLE_STATUS;

WiFiServer server(88);

void SetupMotors()
{
  // initialize digital pin 13 as an output.
  pinMode(M1, OUTPUT);   
  pinMode(M2, OUTPUT); 
  pinMode(E1, OUTPUT); 
  pinMode(E2, OUTPUT);
  pinMode(13, OUTPUT);
}

void setup() {
  // initialize serial:
  Serial.begin(9600);
  Serial.println("Attempting to connect to WPA network...");
  Serial.print("SSID: ");
  Serial.println(ssid);

  status = WiFi.begin(ssid, pass);
  if ( status != WL_CONNECTED) { 
    Serial.println("Couldn't get a wifi connection");
    while(true);
  } 
  else {
    server.begin();
    Serial.print("Connected to wifi. My address:");
    IPAddress myAddress = WiFi.localIP();
    Serial.println(myAddress);
  }

  SetupMotors();
}

// Motors functions ========================================================================================================

void MoveForward(int time, int speed)
{
  digitalWrite(E1, HIGH);
  digitalWrite(E2, HIGH);

  analogWrite(M1,speed);
  analogWrite(M2, speed);

  delay(time);
}

void MoveBack(int time, int speed)
{
  digitalWrite(E1, HIGH);
  digitalWrite(E2, HIGH);
  
  analogWrite(M1, speed);
  analogWrite(M2, speed);

  delay(time);
}

void TurnRight(int time, int speed)
{
  digitalWrite(E1, HIGH);
  digitalWrite(E2, HIGH);
  
  analogWrite(M1,speed);
  analogWrite(M2, 255 - speed);
}

void TurnLeft(int time, int speed)
{
  digitalWrite(E1, HIGH);
  digitalWrite(E2, HIGH);
  
  analogWrite(M2,speed);
  analogWrite(M1, 255 - speed);
}

void Stop()
{
  digitalWrite(E1, LOW);
  digitalWrite(E2, LOW);
}

// End motor functions =====================================================================================================

void DoCurrentCommand(String command)
{
  int time = 1;
  if (command == "FastForward")
  {
      MoveForward(time, 55);
  }
  else if (command == "SlowForward")
  {
     MoveForward(time, 105);
  }
  else if (command == "Stop")
  {
    Stop();
  }
  else if (command == "SlowBack")
  {
    MoveBack(time, 150);
  }
  else if (command == "FastBack")
  {
     MoveBack(time, 200);
  }
  else if (command == "FastLeft")
  {
    TurnLeft(time, 55);
  }
  else if (command == "SlowLeft")
  {
    TurnLeft(time, 75);
  }
  else if (command == "SlowRight")
  {
    TurnRight(time, 75);
  }
  else if (command == "FastRight")
  {
    TurnRight(time, 55);
  }
}


void loop() {
  // listen for incoming clients
  Serial.println("waiting for clients....");
  String readString = String();
  String currentCommand = String();
  
  long count=0;
  bool isHigh = false;
  for(;;)
  {
    count++;
    if (count==100000)
    {
       if (isHigh)
       {
          digitalWrite(13, HIGH);
          isHigh=false;
          count=0;
       }
       else
       {
          digitalWrite(13, LOW);
          isHigh=true;
          count=0;
       }
      
    }
    
    WiFiClient client = server.available();
    if (client) {
      if (client.available() > 0)  {
        char c = client.read();
        if (c == '\n')
        {
          Serial.println(readString);
          if (readString == "connect")
          {
             Serial.println("connected");
             client.flush();
             client.println("connected");
          } else 
          { currentCommand = readString; }
          readString = "";
        }
        else
        {
          readString += c;
        }
      }
  
      if (!client.connected()) {
        Serial.println("Client disconnected");
        // close the connection:
        client.stop();
        currentCommand = "Stop";
        Stop();
     }

     DoCurrentCommand(currentCommand);
    }     
  }
}
 
