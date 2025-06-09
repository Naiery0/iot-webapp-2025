# IoT 더미데이터 생성 프로그램
# 현재일부터 10000일 전을 시작으로 하루에 데이터 100개씩

import mysql.connector
import random
from datetime import datetime, timedelta

conn = mysql.connector.connect(
    host='localhost',
    database='smarthome',
    user='root',
    password='12345',

)
cursor = conn.cursor()

start_date = datetime(1998, 1, 20)
records_per_day = 100
total_days=10000

insert_query='''
INSERT INTO iot_datas (sensing_dt, loc_id, temp, humid)
VALUES (%s, %s, %s, %s)
'''

batch_size = 1000
batch = []

for day in range(total_days):
    date= start_date + timedelta(days=day)
    for i in range(records_per_day):
        timestamp = date + timedelta(minutes=15 * i)
        temp = round(random.uniform(19.0, 28.0), 1)
        humid = round(random.uniform(30.0, 60.0), 2)
        batch.append((timestamp, 'DINNING', temp, humid))

        if len(batch) >= batch_size:
            cursor.executemany(insert_query, batch)  # 한꺼번에 배치사이즈만큼 데이터 삽입
            conn.commit()
            batch.clear()
    print(f'100번 완료 : {day}')
## 남은 배치리스트가 있으면
if batch:
    cursor.executemany(insert_query, batch)
    conn.commit()
    batch.clear()  # 다 끝나서 굳이 클리어 안해도 됨

## 리소스 해제
cursor.close()
conn.close()
print('더미데이터 삽입 완료!!!')

