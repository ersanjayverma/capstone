package com.capstone.Arogya.dto;

import lombok.*;

@Getter 
@Setter 
@NoArgsConstructor 
@AllArgsConstructor 
@Builder
public class SubmitAnswerDto {
    private Long questionId;
    private String answerText;   // for STRING
    private Integer answerInt;   // for INT
    private Long selectedOptionId; // for MCQ
}
