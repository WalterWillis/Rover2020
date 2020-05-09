/*
 MotorDriverRev3
 Runs a DC motor via the Motor Driver Shield PCB.
 Part of the Research Rover Project for BRCTC.

 Test1- Nakyea
  motors 1 and 2 will move at speed as low as 35 (12V and 180mA)
  motors 3 and 4 will move at speed as low as 100 (12V and 150mA) 
  2V less forwarding to motors 3 and 4 compared to motors 1 and 2
  all together at lowest working speed (12V and 230mA)
  when all motors are at the same speed motors 3 and 4 are slower than motors 1 and 2
  motors 1 and 2 at speed 200 and motors 3 and 4 at speed 250 the voltage is closer together

 Test2 - Walter, Chris & Corey
  Motors 1 & 2 require a speed increase of 25%
  Motors 2 & 3 are configured incorrectly. Swap them in the Left and Right functions.
 */

#include <AFMotor.h>
#include <Wire.h>

// Motor pulse drive frequency and speed.
AF_DCMotor motor1(1, MOTOR12_2KHZ);
AF_DCMotor motor2(2, MOTOR12_2KHZ);
AF_DCMotor motor3(3, MOTOR12_2KHZ);
AF_DCMotor motor4(4, MOTOR12_2KHZ);


//AF_DCMotor motor1(1, MOTOR12_64KHZ); // create motor #1, 64KHz pwm
//AF_DCMotor motor2(2, MOTOR12_64KHZ); // motor #2
//AF_DCMotor motor3(3, MOTOR12_64KHZ); // motor #3
//AF_DCMotor motor4(4, MOTOR12_64KHZ); // motor #4

void setup() {
  Wire.begin(0x8);            // join i2c bus with address #8
  Wire.onReceive(driveEvent); // register event
  Serial.begin(9600); 
}

void loop() {        
  delay(100);
}

void driveEvent(int num) {
  while (Wire.available()) { 
    char direction = Wire.read(); 
    int speed = Wire.read();

    switch(direction){
      case 0x00:
        forward(speed);
        break;
      case 0x01:
        backup(speed);
        break;
      case 0x02:
        left(speed);
        break;
      case 0x03:
        right(speed);
        break;
      case 0x04:
        halt();
        break;
      default:
        break;
    }
  }
}

void forward(int speed) {
  Serial.println("All motors ahead - fast");  
  motor1.run(FORWARD);
  motor2.run(FORWARD);
  motor3.run(FORWARD);
  motor4.run(FORWARD);  
  motor1.setSpeed(speed); 
  motor2.setSpeed(speed);
  motor3.setSpeed(speed);
  motor4.setSpeed(speed);
}
void backup(int speed) {
  Serial.println("All motors backward - fast");
  motor1.run(BACKWARD);
  motor2.run(BACKWARD);
  motor3.run(BACKWARD);
  motor4.run(BACKWARD);
  motor1.setSpeed(speed); 
  motor2.setSpeed(speed);
  motor3.setSpeed(speed);
  motor4.setSpeed(speed);
}

void left(int speed) {
  Serial.println("Pivot left");
  motor1.run(BACKWARD);
  motor2.run(FORWARD);
  motor3.run(BACKWARD);
  motor4.run(FORWARD); 
  motor1.setSpeed(speed); 
  motor2.setSpeed(speed);  
  motor3.setSpeed(speed); 
  motor4.setSpeed(speed);
}

void right(int speed) {
  Serial.println("Pivot right");
  motor1.run(FORWARD);
  motor2.run(BACKWARD);
  motor3.run(FORWARD);
  motor4.run(BACKWARD); 
  motor1.setSpeed(speed); 
  motor2.setSpeed(speed);  
  motor3.setSpeed(speed); 
  motor4.setSpeed(speed);
}

void halt() {
  uint8_t i;
  for(i=255; i!=0; i--) {
    motor1.setSpeed(i); 
    motor2.setSpeed(i); 
    motor3.setSpeed(i); 
    motor4.setSpeed(i);
    delay(10);
    }
   Serial.println("All motors stop");
   motor1.run(RELEASE);
   motor2.run(RELEASE);
   motor3.run(RELEASE);
   motor4.run(RELEASE);   
}
