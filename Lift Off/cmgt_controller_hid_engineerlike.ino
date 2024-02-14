/*
  program Game Controller as HID

  Upload this file to your Arduino,
  you can check with Notepad / Word or any other text based program
  the output of the joystick button (space)
*/

#include "Keyboard.h"
#include "Mouse.h"
#include "Arduino_LSM6DS3.h"
#include "MadgwickAHRS.h"

Madgwick filter;

// -------
// DEFINES
// -------
#define NR_OF_ARRAY_ELEMENTS( array ) ( sizeof( array ) / sizeof( typeof( *array ) ) )


// ---------
// CONSTANTS
// ---------
enum pinModes {
    UNUSED_PIN     = -1,
    DIGITAL_INPUT  =  0,
    ANALOG_INPUT   =  1,
    DIGITAL_OUTPUT =  2,
    ANALOG_OUTPUT  =  3,
    SERVO          =  4
};

const int JOYSTICK_BUTTON_PIN =  2;
const int     LEFT_BUTTON_PIN =  3;
const int       UP_BUTTON_PIN =  4;
const int     DOWN_BUTTON_PIN =  5;
const int    RIGHT_BUTTON_PIN =  6;
const int    FIRE2_BUTTON_PIN =  7;
const int    FIRE1_BUTTON_PIN =  8;

int PIN_MODES[] {
     ANALOG_INPUT,     // pin 00, TX
     ANALOG_INPUT,     // pin 01, RX
    DIGITAL_INPUT,   // pin 02, joystick button
    DIGITAL_INPUT,   // pin 03, left button
    DIGITAL_INPUT,   // pin 04, up button
    DIGITAL_INPUT,   // pin 05, down button
    DIGITAL_INPUT,   // pin 06, right button
    DIGITAL_INPUT,   // pin 07, fire 2 button
    DIGITAL_INPUT,   // pin 08, fire 1 button
     UNUSED_PIN,     // pin 09, servo, does not properly do PWM if one or more servo pins are used
     ANALOG_OUTPUT,  // pin 10, right led
     ANALOG_OUTPUT,  // pin 11, middle led
     ANALOG_OUTPUT,  // pin 12, left led
    DIGITAL_OUTPUT,  // pin 13, onboard led
     ANALOG_INPUT,   // pin 14, joystick y axis
     ANALOG_INPUT,   // pin 15, joystick x axis

};

const int MAX_PINS = NR_OF_ARRAY_ELEMENTS( PIN_MODES );
const int xAxis = A0;
const int yAxis = A1;  

float yaw_r = 0.0;
float pitch_r = 0.0;
float roll_r = 0.0;

float yaw = 0.0;
float pitch = 0.0;
float roll = 0.0;

float yaw0 = 0.0;
float pitch0 = 0.0;
float roll0 = 0.0;

float accx_r = 0.0;
float accy_r = 0.0;
float accz_r = 0.0;

float accx = 0.0;
float accy = 0.0;
float accz = 0.0;

float accx0 = 0.0;
float accy0 = 0.0;
float accz0 = 0.0;

// ----------------
// GLOBAL VARIABLES
// ----------------
int   isButtonPressed[MAX_PINS];
int  wasButtonPressed[MAX_PINS];
char button2character[MAX_PINS] = {
    0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
};


 void set_all_pinmodes()
{
    for ( int pin = 0 ; pin < MAX_PINS ; pin++ ) {
        if ( PIN_MODES[pin] != UNUSED_PIN ) {
            pinMode( pin , PIN_MODES[pin] );
        }
    }
}


 void read_all_inputs()
{
    for ( int pin = 0 ; pin < MAX_PINS ; pin++ ) {
        if ( PIN_MODES[pin] == DIGITAL_INPUT ) {
            wasButtonPressed[pin] = isButtonPressed[pin];
            isButtonPressed[pin] = digitalRead( pin );
        }
    }
}


 void write_keyboard_outputs()
{
    for ( int pin = 0 ; pin < MAX_PINS ; pin++ ) {
        if ( PIN_MODES[pin] == DIGITAL_INPUT ) {
            if ( isButtonPressed[pin] && ! wasButtonPressed[pin] ) {
                Keyboard.press( button2character[ pin ] );
            } else  if ( ! isButtonPressed[pin] && wasButtonPressed[pin] ) {
                Keyboard.release( button2character[ pin ] );   
            }
        }
    }
}

void setup() {
    delay( 3000 );     // to make reprogramming etc. easier
    
    set_all_pinmodes();

    Serial.begin( 115200 );
    Mouse.begin();
    // attempt to start the IMU:
    if (!IMU.begin()) {
      Serial.println("Failed to initialize IMU");
      // stop here if you can't access the IMU:
      while (true);
    }
  
    // start the filter to run at the sample rate of the IMU
    filter.begin(IMU.accelerationSampleRate());
}


void loop() {
    read_all_inputs();
    write_keyboard_outputs();
    int xReading = analogRead(A1);
    int yReading = analogRead(A0);
    if (xReading>750 || xReading<250 || yReading>750 || yReading<250)
    {
        xReading = map(xReading, 0, 1023, -12, 12);
        yReading = map(yReading, 0, 1023, -12, 12);
        Mouse.move(xReading, yReading);
    }
    // if (digitalRead(4) == HIGH)
    //   Mouse.click();
    // else
    //   Mouse.release();
    if (digitalRead(7) == HIGH)
    {
      yaw0 = yaw_r;
      pitch0 = pitch_r;
      roll0 = roll_r;

      accx0 = accx_r;
      accy0 = accy_r;
      accz0 = accz_r - 1;

    }

  float xAcc, yAcc, zAcc;
  float xGyro, yGyro, zGyro;


  // check if the IMU is ready to read:
  if (IMU.accelerationAvailable() && IMU.gyroscopeAvailable()) {
    // read accelerometer &and gyrometer:
    IMU.readAcceleration(xAcc, yAcc, zAcc);
    IMU.readGyroscope(xGyro, yGyro, zGyro);


    // update the filter, which computes orientation:
    filter.updateIMU(xGyro, yGyro, zGyro, xAcc, yAcc, zAcc);

    // print the yaw (heading), pitch and roll
    yaw_r   = filter.getYaw();
    pitch_r = filter.getPitch();
    roll_r  = filter.getRoll();

    accx_r = xAcc;
    accy_r = yAcc;
    accz_r = zAcc;

    yaw = yaw_r - yaw0;
    pitch = pitch_r - pitch0;
    roll = roll_r - roll0;

    accx = accx_r - accx0;
    accy = accy_r - accy0;
    accz = accz_r - accz0;

    Serial.println("start");
    //Serial.println("Orientation: ");
    Serial.println(yaw);
    //Serial.print(",");
    Serial.println(pitch);
    //Serial.print(",");
    Serial.println(roll);
    Serial.println(accx);
    Serial.println(accy);
    Serial.println(accz);

    Serial.println(digitalRead(4) == HIGH);
    Serial.println(digitalRead(7) == HIGH);
  }
  
  // if (pitch < -20)
  //   Keyboard.press('d');
  // else
  //   Keyboard.release('d');

  // if (pitch > 20)
  //   Keyboard.press('a');
  // else
  //   Keyboard.release('a');

  // if (roll < -20)
  //   Keyboard.press('s');
  // else
  //   Keyboard.release('s');

  // if (roll > 20)
  //   Keyboard.press('w');
  // else
  //   Keyboard.release('w');
}

