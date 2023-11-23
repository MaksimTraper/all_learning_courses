import cv2

#загрузка классификатора в объект
face_cascade_db = cv2.CascadeClassifier("haarcascade_frontalface_default.xml")
#объект для захвата видеострима
cap = cv2.VideoCapture(0)
#вечный цикл
while True:
    success, img = cap.read()
    #img = cv2.imread("4.jpg")
    img_gray = cv2.cvtColor(img, cv2.COLOR_BGR2GRAY)#перевод изображения в полутоновый

    faces = face_cascade_db.detectMultiScale(img_gray, 1.1, 19)#функция поиска лиц
    for (x,y,w,h) in faces:
        cv2.rectangle(img, (x,y), (x+w,y+h), (0,255,0),2)#прямоугольник вокруг лица

    cv2.imshow('rez', img)
    #cv2.waitKey()
    #выход из программы по нажатии q
    if cv2.waitKey(10) & 0xff == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
