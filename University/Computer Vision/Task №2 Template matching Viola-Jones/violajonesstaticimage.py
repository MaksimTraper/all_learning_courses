import cv2
#загрузка классификатора в объект
face_cascade_db = cv2.CascadeClassifier("haarcascade_frontalface_default.xml")

img = cv2.imread("5.jpg")
img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)

faces = face_cascade_db.\
    detectMultiScale(img, 1.1, 19)
for (x,y,w,h) in faces:
    cv2.rectangle(img, (x,y),
                  (x+w,y+h), (0,255,0),2)

cv2.imshow('rez', img)
cv2.waitKey()