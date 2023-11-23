import cv2
import dlib

x = []
y = []
i = 0
points = [7, 20, 30, 36, 39, 42, 45] # точки для поиска частей лица
                                     # 7 - край брови; 20 - край челюсти ;30 - кончик носа;
                                     # 36, 39 - углы левого глаза; 42, 45 - углы правого глаза

# Загрузка детектора
detector = dlib.get_frontal_face_detector()
# Загрузка классификатор
predictor = dlib.shape_predictor("shape_predictor_68_face_landmarks.dat")
# загружаем изображение
img = cv2.imread("ronaldu-13.jpg")
# преобразуем изображение в полутоновое
gray = cv2.cvtColor(src=img, code=cv2.COLOR_BGR2GRAY)
# Используем детектор для поиска точек
faces = detector(gray)
for face in faces:
    # Ищем ориентиры
    landmarks = predictor(image=gray, box=face)

    for n in points:
        x.append(landmarks.part(n).x)
        y.append(landmarks.part(n).y)

cv2.line(img, ((x[3]+x[4])//2, y[0]), ((x[3]+x[4])//2, y[1]), (0, 255, 0), 2) #левая побочная линия симметрии
cv2.line(img, ((x[5]+x[6])//2, y[0]), ((x[5]+x[6])//2, y[1]), (0, 255, 0), 2) #правая побочная линия симметрии
cv2.line(img, (x[2], y[0]), (x[2], y[1]//2), (0, 255, 0), 2) #основная линия симметрии
cv2.putText(img, str(((x[5]+x[6])//2)-x[2]), ((x[2]+x[5]+x[6])//3, y[1]), cv2.FONT_HERSHEY_SIMPLEX,
             0.75, (0, 255, 0), 2) # расстояние между правой и средней линиями
cv2.putText(img, str(x[2]-((x[3]+x[4])//2)), ((x[2]+x[3]+x[4])//3, y[1]), cv2.FONT_HERSHEY_SIMPLEX,
             0.75, (0, 255, 0), 2)# расстояние между левой и средней линиями
# Показ изображения
cv2.imshow(winname="Face", mat=img)
cv2.waitKey(delay=0)
cv2.destroyAllWindows()