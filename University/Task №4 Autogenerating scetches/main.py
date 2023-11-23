from AutoGeneratingScetches import AutoGenScetches
import tkinter as tk
def startProgram():
    pDiapazon = int(pDiap.get())
    Generator = AutoGenScetches(amountScetches = 10, intervalRandom = pDiapazon)
    Generator.LoadPhotoAndScetch()
    #Generator.makeViewedScetch()
    Generator.GenerateP()
    Generator.MakeFirstScetchGeneration()
    Generator.MakeSecondScetchGeneration()
    Generator.CalculateSSIM()
    Generator.ShowResults()

startwindow = tk.Tk()
startwindow.geometry('250x150')
startwindow.title("Стартовое окно")

mes = tk.Label(startwindow, text='Введите диапазон изменения параметров P:')
btnStartPrg = tk.Button(startwindow, text="Запуск", command=startProgram)
pDiap = tk.Entry(startwindow)

mes.pack(ipadx=20, ipady=20)
pDiap.insert(0, '10')
pDiap.pack()
btnStartPrg.pack(ipadx=10, ipady=10, padx=10, pady=10)

startwindow.mainloop()